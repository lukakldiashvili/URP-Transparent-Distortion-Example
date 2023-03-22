using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

class DistortionPass : ScriptableRenderPass {
	private const string k_GlobalFullSceneColorTexture = "_GlobalFullSceneColorTexture";

	private PassData m_PassData;

	private RTHandle m_tmpRT;

	public void Setup(Action<PassData> passDataOptions, float downSample, in RenderingData renderingData) {
		m_PassData ??= new PassData();

		passDataOptions?.Invoke(m_PassData);

		SetupTmpRt(renderingData, downSample);
	}

	void SetupTmpRt(in RenderingData renderingData, float downSample) {
		RenderTextureDescriptor rtDesc = renderingData.cameraData.cameraTargetDescriptor;
		rtDesc.depthBufferBits = (int) DepthBits.None;

		rtDesc.width  = Mathf.RoundToInt(rtDesc.width  / downSample);
		rtDesc.height = Mathf.RoundToInt(rtDesc.height / downSample);
		
		#if UNITY_2022_1_OR_NEWER
		RenderingUtils.ReAllocateIfNeeded(ref m_tmpRT, rtDesc, name: "_DistortionTmpRT");
		#else
		RenderEmul_2021.ReAllocateIfNeeded(ref m_tmpRT, rtDesc, name: "_DistortionTmpRT");
		#endif
		
		m_PassData.tmpRT = m_tmpRT;
	}

	public void Dispose() {
		m_tmpRT?.Release();
	}

	public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData) {
		ExecutePass(m_PassData, ref renderingData, ref context);
	}

	private static void ExecutePass(PassData passData, ref RenderingData renderingData,
	                                ref ScriptableRenderContext context) {
		var tmpRT = passData.tmpRT;
		
		if (renderingData.cameraData.isPreviewCamera)
			return;

		// if is scene camera and we want to disable in scene view
		if (renderingData.cameraData.isSceneViewCamera && passData.disableInSceneView)
			return;

		CommandBuffer cmd =
			#if UNITY_2022_1_OR_NEWER
			renderingData.commandBuffer;
			#else
			CommandBufferPool.Get();
		#endif

		var cameraData = renderingData.cameraData;

		using (new ProfilingScope(cmd, passData.profilingSampler)) {
			ProcessEffect(ref context);
		}


		void ProcessEffect(ref ScriptableRenderContext context) {
			var source =
				#if UNITY_2022_1_OR_NEWER
					passData.isBeforeTransparents
					? cameraData.renderer.GetCameraColorBackBuffer(cmd)
					: cameraData.renderer.cameraColorTargetHandle;
				#else
				cameraData.renderer.cameraColorTarget;
			#endif
			
			cmd.Blit(source, tmpRT);

			cmd.SetGlobalTexture(k_GlobalFullSceneColorTexture, tmpRT);

			context.ExecuteCommandBuffer(cmd);
			cmd.Clear();
		}
	}

	internal class PassData {
		internal bool disableInSceneView;

		public ProfilingSampler profilingSampler;
		public RTHandle tmpRT;
	}
}