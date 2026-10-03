#if !UNITY_6000_4_OR_NEWER
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.Rendering;

namespace jp.lilxyzw.shadercore
{
    internal class DynamicVariantStripper : IPreprocessShaders
    {
        public int callbackOrder => 0;

        public void OnProcessShader(Shader shader, ShaderSnippetData snippet, IList<ShaderCompilerData> data)
        {
            // Strip dynamic_branch
            // https://issuetracker.unity.com/issues/9284/dynamic-branching-generates-shader-variants-when-building-the-project
            Strip(data, false, ShaderUtil.GetPassKeywords(shader, snippet.pass, snippet.shaderType).Where(k => k.isDynamic).Select(k => new ShaderKeyword(k.name)).ToArray());
        }

        private static void Strip(IList<ShaderCompilerData> data, bool stripAll, params ShaderKeyword[] keywords)
        {
            foreach (var keyword in keywords)
            {
                if (!stripAll && data.All(d => d.shaderKeywordSet.IsEnabled(keyword))) continue;
                for (int i = data.Count - 1; i >= 0; i--)
                {
                    if (data[i].shaderKeywordSet.IsEnabled(keyword))
                        data.RemoveAt(i);
                }
            }
        }
    }
}
#endif
