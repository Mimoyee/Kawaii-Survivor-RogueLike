// 着色器定义开始
Shader "Custom/BasicLighting_Emission" // 着色器名称，在材质面板显示为Custom/BasicLighting
{
    // 材质属性定义区域，这些属性会在Unity材质面板中显示并可调整
    Properties // 材质属性面板可调参数
    {
        [Toggle(ENABLE_ZWRITE)][Space(5)] _ZWriteEnable ("深度写入", Float) = 0 // 深度写入开关，默认关闭
        // 基础颜色属性，RGBA格式，默认白色
        [Header(BaseColor)][Space(5)]_Color ("颜色", Color) = (1,1,1,1) // 颜色属性，默认白色
        _ShadowRange ("暗面范围", Range(0, 1)) = 0.2 // 暗面着色范围，越大暗面越柔和
        _ShadowFeather ("暗面羽化", Range(0, 1)) = 0.1 // 暗面边缘羽化程度，值越大边缘越柔和
        _ShadowColor ("暗面颜色", Color) = (0.5,0.5,0.5,1) // 暗面颜色，默认灰色
        // 高光反射颜色，RGBA格式，默认白色
        [Header(specular)][Space(5)]_SpecColor ("高光颜色", Color) = (1,1,1,1) // 高光颜色，默认白色
        // 高光反射强度参数，控制高光区域大小，值越大高光区域越小
        _Gloss ("高光参数", Range(1, 256)) = 20 // 高光参数，范围1-256，默认20
        // 自发光颜色，RGBA格式，默认黑色(无自发光)
        _EmissionColor ("自发光颜色", Color) = (0,0,0,0) // 自发光颜色，默认无自发光
        [Header(Fresnel)][Space(5)][Toggle]_FresnelEnable ("菲涅尔外发光启用", Float) = 1 // 菲涅尔外发光开关，复选框，0关闭，1启用
        // 菲涅尔效应外发光颜色，RGBA格式，默认白色
        _FresnelColor ("菲涅尔外发光颜色", Color) = (1,1,1,1) // 菲涅尔外发光颜色，默认白色
        // 菲涅尔效应强度，控制边缘发光强度
        _FresnelPower ("菲涅尔强度", Range(0.1, 8)) = 2 // 菲涅尔强度，默认2，可调范围0.1-8
        // 菲涅尔效应边缘软硬度，值越小边缘越锐利
        _FresnelSmoothness ("菲涅尔边缘软硬度", Range(0.01, 2)) = 0.2 // 菲涅尔边缘软硬度，越小越硬，默认0.2
        // 阴影范围参数，控制暗部区域大小
        [Header(Texture)][Space(5)]_MainTex ("主纹理", 2D) = "white" {} // 主纹理贴图
        _EmissionMap ("自发光贴图", 2D) = "white" {} // 自发光贴图
        _AlphaMask ("透明遮罩", 2D) = "white" {} // 透明度控制贴图
           
    }
    // 子着色器定义，一个着色器可以包含多个子着色器以适应不同渲染管线
    SubShader // 定义具体渲染方式
    {
        Tags {"Queue"="Transparent" "RenderType"="Transparent"}
        
        // 第一个渲染通道，处理主平行光(方向光)的渲染
        Pass // 渲染通道
        {
            // 设置渲染通道标签，指定为前向渲染基础光照(主平行光)
            Tags { "LightMode" = "ForwardBase" } // 主平行光，前向渲染基础光照
            Blend SrcAlpha OneMinusSrcAlpha
            ZWrite [_ZWriteEnable]
            // 开始CG着色器代码块
            CGPROGRAM // 开始CG代码块
            // 声明顶点着色器函数
            #pragma vertex vert // 顶点着色器入口
            // 声明片元着色器函数
            #pragma fragment frag // 片元着色器入口
            // 包含Unity内置CG库
            #include "UnityCG.cginc" // Unity通用CG库
            // 包含光照相关函数和宏
            #include "Lighting.cginc" // 光照相关宏
            // 包含阴影相关函数和宏
            #include "AutoLight.cginc" // 阴影相关宏，确保 TRANSFER_SHADOW 可用

            // 定义顶点着色器输入结构体
            struct appdata // 顶点输入结构体，支持点光源和聚光灯时可扩展
            {
                // 顶点位置(模型空间)
                float4 vertex : POSITION; // 顶点位置
                // 顶点法线(模型空间)
                float3 normal : NORMAL; // 法线
                // 纹理坐标
                float2 texcoord : TEXCOORD0; // 纹理坐标
                // 如需切线空间可取消注释
                // float4 tangent : TANGENT; // 如需切线空间可取消注释
            };
            
            // 定义顶点着色器输出/片元着色器输入结构体
            struct v2f { 
                // 裁剪空间位置
                float4 pos : SV_POSITION; 
                // 世界空间法线
                float3 worldNormal : TEXCOORD0; 
                // 世界空间位置
                float3 worldPos : TEXCOORD1; 
                // 阴影坐标
                float4 shadowCoord : TEXCOORD2;
                // 纹理坐标
                float2 uv : TEXCOORD3; 
            }; // 顶点到片元结构体，增加阴影坐标和纹理坐标

            // 声明从Properties获取的变量
            float4 _Color; // 颜色变量
            float4 _EmissionColor; // 自发光颜色变量
            float _Gloss; // 高光参数
            float4 _FresnelColor; // 菲涅尔外发光颜色变量
            float _FresnelPower; // 菲涅尔强度变量
            float _FresnelEnable; // 菲涅尔外发光开关变量
            float _FresnelSmoothness; // 菲涅尔边缘软硬度变量
            sampler2D _MainTex; // 主纹理
            sampler2D _EmissionMap; // 自发光贴图
            sampler2D _AlphaMask; // 透明遮罩
            float _ShadowRange; // 暗面范围参数
            float _ShadowFeather; // 暗面边缘羽化程度
            float4 _ShadowColor; // 暗面颜色变量

            // 顶点着色器函数
            v2f vert(appdata v) // 顶点着色器
            {
                v2f o;
                // 将顶点位置从模型空间转换到裁剪空间
                o.pos = UnityObjectToClipPos(v.vertex); // 转换到裁剪空间
                // 将法线从模型空间转换到世界空间
                o.worldNormal = UnityObjectToWorldNormal(v.normal); // 转换法线到世界空间
                // 将顶点位置从模型空间转换到世界空间
                o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz; // 顶点世界空间位置
                // 传递纹理坐标
                o.uv = v.texcoord; // 传递纹理坐标
                // 初始化阴影坐标
                o.shadowCoord = 0; // 默认初始化，防止未赋值警告
                // 计算并传递阴影坐标
                TRANSFER_SHADOW(o); // 正确传递阴影坐标
                return o;
            }

            // 片元着色器函数
            fixed4 frag(v2f i) : SV_Target // 片元着色器
            {
                // 采样贴图
                fixed4 mainTex = tex2D(_MainTex, i.uv); // 主纹理采样
                fixed4 emissionTex = tex2D(_EmissionMap, i.uv); // 自发光贴图采样
                fixed alphaMask = tex2D(_AlphaMask, i.uv).r; // 透明遮罩采样

                // 归一化世界空间法线
                float3 N = normalize(i.worldNormal); // 法线归一化

                // 应用贴图颜色
                fixed3 baseColor = _Color.rgb * mainTex.rgb; // 基础颜色乘以主纹理
                fixed3 emissionColor = _EmissionColor.rgb * emissionTex.rgb; // 自发光颜色乘以自发光贴图
                // 获取主光源方向(世界空间)
                float3 L = normalize(_WorldSpaceLightPos0.xyz); // 光源方向
                // 计算视线方向(世界空间)
                float3 V = normalize(_WorldSpaceCameraPos - i.worldPos); // 视线方向
                // 计算半角向量(用于Blinn-Phong高光)
                float3 H = normalize(L + V); // 半角向量

                // Lambert漫反射计算
                // Lambert Diffuse // Lambert漫反射
                // 使用smoothstep控制暗部范围
                float diffuseFactor = smoothstep(_ShadowRange - _ShadowFeather, _ShadowRange + _ShadowFeather, dot(N, L)); // 漫反射分量，暗面范围和羽化可调
                float3 diffuse = lerp(_ShadowColor.rgb, fixed3(1,1,1), diffuseFactor); // 根据暗面系数混合暗面颜色

                // Blinn-Phong高光计算
                // Blinn-Phong Specular // Blinn-Phong高光
                // 计算高光强度
                float specular = pow(max(0, dot(N, H)), _Gloss); // 高光分量

                // 合成最终光照颜色
                // Combine // 合成最终光照
                // 计算环境光分量
                fixed3 ambient = UNITY_LIGHTMODEL_AMBIENT.rgb * baseColor; // 环境光分量乘以基础颜色
                // 组合环境光、漫反射和高光
                fixed3 lighting = ambient + 
                                _LightColor0.rgb * baseColor * diffuse + 
                                _LightColor0.rgb * _SpecColor.rgb * specular; // 漫反射+高光
                // 加入自发光分量
                lighting += emissionColor; // 加入自发光分量

                // 菲涅尔外发光效果计算
                // Fresnel 外发光（带开关和软硬控制）
                if (_FresnelEnable > 0.5) {
                    // 计算法线与视线夹角
                    float nv = saturate(dot(N, V)); // 法线与视线夹角
                    // 计算菲涅尔效应强度
                    float fresnel = pow(1.0 - nv, _FresnelPower) * smoothstep(0, _FresnelSmoothness, 1.0 - nv); // 菲涅尔项，边缘更亮，软硬可调
                    // 加入菲涅尔外发光颜色
                    lighting += _FresnelColor.rgb * fresnel; // 加入菲涅尔外发光
                }

                // 阴影计算
                float shadow = SHADOW_ATTENUATION(i); // 获取阴影衰减
                // 应用阴影到最终光照
                lighting *= shadow; // 乘以阴影

                // 返回最终颜色(应用透明遮罩)
                return fixed4(lighting, alphaMask); // 返回最终颜色
            }
            // 结束CG代码块
            ENDCG // 结束CG代码块
        }
        // 第二个渲染通道，处理附加光源(点光源/聚光灯)的渲染
        Pass // 点光源/聚光灯叠加通道
        {
            // 设置渲染通道标签，指定为前向渲染附加光照(点光源/聚光灯)
            Tags { "LightMode" = "ForwardAdd" } // 前向叠加光照（点光源/聚光灯）
            // 设置混合模式为叠加(One One)
            Blend One One // 叠加混合模式
            // 开始CG着色器代码块
            CGPROGRAM // 开始CG代码块
            // 启用点光源/聚光灯阴影和衰减宏
            #pragma multi_compile_fwdadd_fullshadows // 支持点光源/聚光灯阴影和衰减宏
            // 包含阴影相关函数和宏
            #include "AutoLight.cginc" // 引入阴影相关宏（提前到结构体声明前）
            // 声明顶点着色器函数
            #pragma vertex vert // 顶点着色器入口
            // 声明片元着色器函数(使用不同名称避免冲突)
            #pragma fragment frag_add // 片元着色器入口
            // 包含Unity内置CG库
            #include "UnityCG.cginc" // Unity通用CG库
            // 包含光照相关函数和宏
            #include "Lighting.cginc" // 光照相关宏

            // 声明纹理变量
            sampler2D _MainTex; // 主纹理
            sampler2D _EmissionMap; // 自发光贴图
            sampler2D _AlphaMask; // 透明遮罩

            // 定义顶点着色器输入结构体
            struct appdata { // 顶点输入结构体
                // 顶点位置(模型空间)
                float4 vertex : POSITION; // 顶点位置
                // 顶点法线(模型空间)
                float3 normal : NORMAL; // 法线
                // 纹理坐标
                float2 texcoord : TEXCOORD0; // 纹理坐标
            };
            
            // 定义顶点着色器输出/片元着色器输入结构体
            struct v2f { // 顶点到片元结构体
                // 裁剪空间位置
                float4 pos : SV_POSITION; // 裁剪空间位置
                // 世界空间法线
                float3 worldNormal : TEXCOORD0; // 世界空间法线
                // 世界空间位置
                float3 worldPos : TEXCOORD1; // 世界空间位置
                // 阴影坐标
                float4 shadowCoord : TEXCOORD2; // 阴影坐标，供阴影宏使用
                // 纹理坐标
                float2 uv : TEXCOORD3; // 纹理坐标
            };

            // 声明从Properties获取的变量
            float4 _Color; // 颜色变量
            float4 _EmissionColor; // 自发光颜色变量
            float _Gloss; // 高光参数
            float4 _FresnelColor; // 菲涅尔外发光颜色变量
            float _FresnelPower; // 菲涅尔强度变量
            float _FresnelEnable; // 菲涅尔外发光开关变量
            float _FresnelSmoothness; // 菲涅尔边缘软硬度变量
            float _ShadowRange; // 暗面范围参数

            // 顶点着色器函数
            v2f vert(appdata v) // 顶点着色器
            {
                v2f o;
                // 将顶点位置从模型空间转换到裁剪空间
                o.pos = UnityObjectToClipPos(v.vertex); // 转换到裁剪空间
                // 将法线从模型空间转换到世界空间
                o.worldNormal = UnityObjectToWorldNormal(v.normal); // 转换法线到世界空间
                // 将顶点位置从模型空间转换到世界空间
                o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz; // 顶点世界空间位置
                // 传递纹理坐标
                o.uv = v.texcoord; // 传递纹理坐标
                // 初始化阴影坐标
                o.shadowCoord = 0; // 默认初始化，防止未赋值警告
                // 计算并传递阴影坐标
                TRANSFER_SHADOW(o); // 传递阴影坐标（正确用法）
                return o;
            }

            // 片元着色器函数(附加光源)
            fixed4 frag_add(v2f i) : SV_Target // 片元着色器
            {
                // 采样贴图
                fixed4 mainTex = tex2D(_MainTex, i.uv); // 主纹理采样
                fixed4 emissionTex = tex2D(_EmissionMap, i.uv); // 自发光贴图采样
                fixed alphaMask = tex2D(_AlphaMask, i.uv).r; // 透明遮罩采样

                // 归一化世界空间法线
                float3 N = normalize(i.worldNormal); // 法线归一化

                // 应用贴图颜色
                fixed3 baseColor = _Color.rgb * mainTex.rgb; // 基础颜色乘以主纹理
                fixed3 emissionColor = _EmissionColor.rgb * emissionTex.rgb; // 自发光颜色乘以自发光贴图
                // 计算光源到像素的向量(点光源)
                // 点光源方向和距离
                float3 lightVec = _WorldSpaceLightPos0.xyz - i.worldPos; // 光源到像素的向量
                // 计算光源方向(点光源)
                float3 L = normalize(lightVec); // 点光源方向
                // 计算视线方向
                float3 V = normalize(_WorldSpaceCameraPos - i.worldPos); // 视线方向
                // 计算半角向量(用于Blinn-Phong高光)
                float3 H = normalize(L + V); // 半角向量

                // 计算漫反射分量(使用smoothstep控制暗部范围)
                float diffuse = smoothstep(_ShadowRange, 1.0, dot(N, L)); // 漫反射分量，暗面范围可调
                // 计算高光分量
                float specular = pow(max(0, dot(N, H)), _Gloss); // 高光分量

                // 计算点光源距离衰减
                // 点光源距离衰减（Unity标准参数，可调整）
                float distanceSqr = dot(lightVec, lightVec); // 距离平方
                // 使用Unity标准衰减公式
                float atten = 1.0 / (1.0 + 0.09 * sqrt(distanceSqr) + 0.032 * distanceSqr); // 距离衰减

                // 计算漫反射颜色(包含衰减)
                fixed3 diffuseColor = _LightColor0.rgb * baseColor * diffuse * atten; // 漫反射颜色
                // 计算高光颜色(包含衰减)
                fixed3 specColor = _LightColor0.rgb * _SpecColor.rgb * specular * atten; // 高光颜色
                // 自发光分量
                fixed3 emission = emissionColor; // 自发光分量

                // 菲涅尔外发光效果计算
                // Fresnel 外发光（带开关和软硬控制）
                fixed3 fresnelColor = 0;
                if (_FresnelEnable > 0.5) {
                    // 计算法线与视线夹角
                    float nv = saturate(dot(N, V)); // 法线与视线夹角
                    // 计算菲涅尔效应强度
                    float fresnel = pow(1.0 - nv, _FresnelPower) * smoothstep(0, _FresnelSmoothness, 1.0 - nv); // 菲涅尔项，边缘更亮，软硬可调
                    // 计算菲涅尔外发光颜色
                    fresnelColor = _FresnelColor.rgb * fresnel; // 菲涅尔外发光分量
                }

                // 返回最终颜色(叠加模式，应用透明遮罩)
                return fixed4((diffuseColor + specColor + emission + fresnelColor).rgb, alphaMask); // 返回最终颜色
            }
            // 结束CG代码块
            ENDCG // 结束CG代码块
        }
        // 第三个渲染通道，处理阴影投射
        Pass // 阴影投射通道
        {
            // 设置通道名称
            Name "ShadowCaster"
            // 设置渲染通道标签，指定为阴影投射
            Tags { "LightMode" = "ShadowCaster" }
            // 开始CG着色器代码块
            CGPROGRAM
            // 声明顶点着色器函数
            #pragma vertex vert
            // 声明片元着色器函数
            #pragma fragment frag
            // 启用阴影投射宏
            #pragma multi_compile_shadowcaster
            // 包含Unity内置CG库
            #include "UnityCG.cginc"
            
            // 定义顶点着色器输入结构体
            struct appdata {
                // 顶点位置(模型空间)
                float4 vertex : POSITION;
                // 顶点法线(模型空间)
                float3 normal : NORMAL;
            };
            
            // 定义顶点着色器输出结构体
            struct v2f {
                // 裁剪空间位置
                float4 pos : SV_POSITION;
            };
            
            // 顶点着色器函数
            v2f vert(appdata v) {
                v2f o;
                // 将顶点位置从模型空间转换到裁剪空间
                o.pos = UnityObjectToClipPos(v.vertex);
                return o;
            }
            
            // 片元着色器函数
            float4 frag(v2f i) : SV_Target {
                // 返回0表示完全阴影
                return 0;
            }
            // 结束CG代码块
            ENDCG
        }
    }
}