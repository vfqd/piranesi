using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Experimental.AssetImporters;
using UnityEngine;
using UnityEngine.UI;

namespace AutoImporterForAseprite
{
    public class AseImportContextWorkerPro : AseImportContextWorker
    {
        public AseImportContextWorkerPro(AssetImportContext asefileContext) : base(asefileContext)
        {
        }

        public override AnimationClip GenerateAnimation(FrameTag frameTag, IList<FrameElement> allFrames,
            IList<Sprite> allSprites, AnimationOptions options)
        {
            if (Config.IsLiteVersion)
            {
                return base.GenerateAnimation(frameTag, allFrames, allSprites, options);
     
            }

            var clip = new AnimationClip();
            var curveBinding = new EditorCurveBinding
            {
                path = options.relativePath,
                type = options.componentType == ComponentType.Image ? typeof(Image) : typeof(SpriteRenderer),
                propertyName = "m_Sprite"
            };

            var keyFrames = new List<ObjectReferenceKeyframe>();

            float time = 0;
            float lastCustomCurveFrameTime = 0;
            float firstCustomCurveFrameTime = 0;
            if (options.useCustomCurve && options.customCurve != null)
            {
                lastCustomCurveFrameTime = options.customCurve.keys.Last().time;
                firstCustomCurveFrameTime = options.customCurve.keys.First().time;
            }

            for (var i = frameTag.@from; i <= frameTag.to; i++)
            {
                keyFrames.Add(new ObjectReferenceKeyframe
                {
                    time = time,
                    value = allSprites[options.direction == Direction.Reverse ? frameTag.to - i + frameTag.@from : i]
                });

                time += EvaluateFrameDuration(frameTag, allFrames, options, i, lastCustomCurveFrameTime,
                    firstCustomCurveFrameTime);
            }

            if (options.direction == Direction.PingPong)
            {
                for (var i = frameTag.@from; i <= frameTag.to; i++)
                {
                    keyFrames.Add(new ObjectReferenceKeyframe
                    {
                        time = time,
                        value = allSprites[frameTag.to - i + frameTag.@from]
                    });

                    time += EvaluateFrameDuration(frameTag, allFrames, options, i, lastCustomCurveFrameTime,
                        firstCustomCurveFrameTime);
                }
            }

            keyFrames.Add(new ObjectReferenceKeyframe
            {
                time = time - (1f / clip.frameRate),
                value = options.direction == Direction.Reverse || options.direction == Direction.PingPong
                    ? allSprites[frameTag.@from]
                    : allSprites[frameTag.to]
            });


            clip.name = frameTag.name;

            if (this.AseFileNoExt != null && clip.name.StartsWith(AseFileNoExt + "_") == false)
            {
                clip.name = AseFileNoExt + "_" + clip.name;
            }

            if (options.loopTime)
            {
                var set = AnimationUtility.GetAnimationClipSettings(clip);
                set.loopTime = true;
                AnimationUtility.SetAnimationClipSettings(clip, set);
            }

            AnimationUtility.SetObjectReferenceCurve(clip, curveBinding, keyFrames.ToArray());
            return clip;
        }

     

        private static float EvaluateFrameDuration(FrameTag frameTag, IList<FrameElement> allFrames,
            AnimationOptions options, int i,
            float lastCustomCurveFrameTime, float firstCustomCurveFrameTime)
        {
            if (options.useCustomCurve && options.customCurve != null)
            {
                var r = (i - frameTag.@from) / (float)(frameTag.to - frameTag.@from);
                var t = (r * (lastCustomCurveFrameTime - firstCustomCurveFrameTime)) + firstCustomCurveFrameTime;
                return options.customCurve.Evaluate(t);
            }
            else
            {
                return allFrames[i].duration / 1000f;
            }
        }
    }
}