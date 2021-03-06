using BeardedManStudios.Forge.Networking;
using BeardedManStudios.Forge.Networking.Unity;
using UnityEngine;

namespace BeardedManStudios.Forge.Networking.Generated
{
	[GeneratedRPC("{\"types\":[[\"uint\", \"int\"][\"uint\", \"uint\"][\"string\"][\"string\", \"uint\", \"Color\"][][\"uint\"]]")]
	[GeneratedRPCVariableNames("{\"types\":[[\"PlayerId\", \"TeamId\"][\"PlayerId\", \"NetworkId\"][\"TeamName\"][\"PlayerName\", \"NetworkId\", \"Color\"][][\"PlayerId\"]]")]
	public abstract partial class PlayerManagerBehavior : NetworkBehavior
	{
		public const byte RPC_ASSIGN_PLAYER_TO_TEAM = 0 + 5;
		public const byte RPC_ASSIGN_NETWORK_PLAYER_TO_PLAYER = 1 + 5;
		public const byte RPC_CREATE_TEAM = 2 + 5;
		public const byte RPC_CREATE_PLAYER = 3 + 5;
		public const byte RPC_SETUP_LOCAL_PLAYER = 4 + 5;
		public const byte RPC_PLAYER_DIED = 5 + 5;
		
		public PlayerManagerNetworkObject networkObject = null;

		public override void Initialize(NetworkObject obj)
		{
			// We have already initialized this object
			if (networkObject != null && networkObject.AttachedBehavior != null)
				return;
			
			networkObject = (PlayerManagerNetworkObject)obj;
			networkObject.AttachedBehavior = this;

			base.SetupHelperRpcs(networkObject);
			networkObject.RegisterRpc("AssignPlayerToTeam", AssignPlayerToTeam, typeof(uint), typeof(int));
			networkObject.RegisterRpc("AssignNetworkPlayerToPlayer", AssignNetworkPlayerToPlayer, typeof(uint), typeof(uint));
			networkObject.RegisterRpc("CreateTeam", CreateTeam, typeof(string));
			networkObject.RegisterRpc("CreatePlayer", CreatePlayer, typeof(string), typeof(uint), typeof(Color));
			networkObject.RegisterRpc("SetupLocalPlayer", SetupLocalPlayer);
			networkObject.RegisterRpc("PlayerDied", PlayerDied, typeof(uint));

			networkObject.onDestroy += DestroyGameObject;

			if (!obj.IsOwner)
			{
				if (!skipAttachIds.ContainsKey(obj.NetworkId)){
					uint newId = obj.NetworkId + 1;
					ProcessOthers(gameObject.transform, ref newId);
				}
				else
					skipAttachIds.Remove(obj.NetworkId);
			}

			if (obj.Metadata != null)
			{
				byte transformFlags = obj.Metadata[0];

				if (transformFlags != 0)
				{
					BMSByte metadataTransform = new BMSByte();
					metadataTransform.Clone(obj.Metadata);
					metadataTransform.MoveStartIndex(1);

					if ((transformFlags & 0x01) != 0 && (transformFlags & 0x02) != 0)
					{
						MainThreadManager.Run(() =>
						{
							transform.position = ObjectMapper.Instance.Map<Vector3>(metadataTransform);
							transform.rotation = ObjectMapper.Instance.Map<Quaternion>(metadataTransform);
						});
					}
					else if ((transformFlags & 0x01) != 0)
					{
						MainThreadManager.Run(() => { transform.position = ObjectMapper.Instance.Map<Vector3>(metadataTransform); });
					}
					else if ((transformFlags & 0x02) != 0)
					{
						MainThreadManager.Run(() => { transform.rotation = ObjectMapper.Instance.Map<Quaternion>(metadataTransform); });
					}
				}
			}

			MainThreadManager.Run(() =>
			{
				NetworkStart();
				networkObject.Networker.FlushCreateActions(networkObject);
			});
		}

		protected override void CompleteRegistration()
		{
			base.CompleteRegistration();
			networkObject.ReleaseCreateBuffer();
		}

		public override void Initialize(NetWorker networker, byte[] metadata = null)
		{
			Initialize(new PlayerManagerNetworkObject(networker, createCode: TempAttachCode, metadata: metadata));
		}

		private void DestroyGameObject(NetWorker sender)
		{
			MainThreadManager.Run(() => { try { Destroy(gameObject); } catch { } });
			networkObject.onDestroy -= DestroyGameObject;
		}

		public override NetworkObject CreateNetworkObject(NetWorker networker, int createCode, byte[] metadata = null)
		{
			return new PlayerManagerNetworkObject(networker, this, createCode, metadata);
		}

		protected override void InitializedTransform()
		{
			networkObject.SnapInterpolations();
		}

		/// <summary>
		/// Arguments:
		/// uint PlayerId
		/// int TeamId
		/// </summary>
		public abstract void AssignPlayerToTeam(RpcArgs args);
		/// <summary>
		/// Arguments:
		/// uint PlayerId
		/// uint NetworkId
		/// </summary>
		public abstract void AssignNetworkPlayerToPlayer(RpcArgs args);
		/// <summary>
		/// Arguments:
		/// string TeamName
		/// </summary>
		public abstract void CreateTeam(RpcArgs args);
		/// <summary>
		/// Arguments:
		/// string PlayerName
		/// uint NetworkId
		/// Color Color
		/// </summary>
		public abstract void CreatePlayer(RpcArgs args);
		/// <summary>
		/// Arguments:
		/// </summary>
		public abstract void SetupLocalPlayer(RpcArgs args);
		/// <summary>
		/// Arguments:
		/// </summary>
		public abstract void PlayerDied(RpcArgs args);

		// DO NOT TOUCH, THIS GETS GENERATED PLEASE EXTEND THIS CLASS IF YOU WISH TO HAVE CUSTOM CODE ADDITIONS
	}
}