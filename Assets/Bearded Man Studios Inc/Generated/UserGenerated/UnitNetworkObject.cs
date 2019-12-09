using BeardedManStudios.Forge.Networking.Frame;
using BeardedManStudios.Forge.Networking.Unity;
using System;
using UnityEngine;

namespace BeardedManStudios.Forge.Networking.Generated
{
	[GeneratedInterpol("{\"inter\":[0.15,0.15,0,0]")]
	public partial class UnitNetworkObject : NetworkObject
	{
		public const int IDENTITY = 10;

		private byte[] _dirtyFields = new byte[1];

		#pragma warning disable 0067
		public event FieldChangedEvent fieldAltered;
		#pragma warning restore 0067
		[ForgeGeneratedField]
		private Vector3 _Position;
		public event FieldEvent<Vector3> PositionChanged;
		public InterpolateVector3 PositionInterpolation = new InterpolateVector3() { LerpT = 0.15f, Enabled = true };
		public Vector3 Position
		{
			get { return _Position; }
			set
			{
				// Don't do anything if the value is the same
				if (_Position == value)
					return;

				// Mark the field as dirty for the network to transmit
				_dirtyFields[0] |= 0x1;
				_Position = value;
				hasDirtyFields = true;
			}
		}

		public void SetPositionDirty()
		{
			_dirtyFields[0] |= 0x1;
			hasDirtyFields = true;
		}

		private void RunChange_Position(ulong timestep)
		{
			if (PositionChanged != null) PositionChanged(_Position, timestep);
			if (fieldAltered != null) fieldAltered("Position", _Position, timestep);
		}
		[ForgeGeneratedField]
		private Quaternion _Rotation;
		public event FieldEvent<Quaternion> RotationChanged;
		public InterpolateQuaternion RotationInterpolation = new InterpolateQuaternion() { LerpT = 0.15f, Enabled = true };
		public Quaternion Rotation
		{
			get { return _Rotation; }
			set
			{
				// Don't do anything if the value is the same
				if (_Rotation == value)
					return;

				// Mark the field as dirty for the network to transmit
				_dirtyFields[0] |= 0x2;
				_Rotation = value;
				hasDirtyFields = true;
			}
		}

		public void SetRotationDirty()
		{
			_dirtyFields[0] |= 0x2;
			hasDirtyFields = true;
		}

		private void RunChange_Rotation(ulong timestep)
		{
			if (RotationChanged != null) RotationChanged(_Rotation, timestep);
			if (fieldAltered != null) fieldAltered("Rotation", _Rotation, timestep);
		}
		[ForgeGeneratedField]
		private float _Health;
		public event FieldEvent<float> HealthChanged;
		public InterpolateFloat HealthInterpolation = new InterpolateFloat() { LerpT = 0f, Enabled = false };
		public float Health
		{
			get { return _Health; }
			set
			{
				// Don't do anything if the value is the same
				if (_Health == value)
					return;

				// Mark the field as dirty for the network to transmit
				_dirtyFields[0] |= 0x4;
				_Health = value;
				hasDirtyFields = true;
			}
		}

		public void SetHealthDirty()
		{
			_dirtyFields[0] |= 0x4;
			hasDirtyFields = true;
		}

		private void RunChange_Health(ulong timestep)
		{
			if (HealthChanged != null) HealthChanged(_Health, timestep);
			if (fieldAltered != null) fieldAltered("Health", _Health, timestep);
		}
		[ForgeGeneratedField]
		private byte _UnitState;
		public event FieldEvent<byte> UnitStateChanged;
		public Interpolated<byte> UnitStateInterpolation = new Interpolated<byte>() { LerpT = 0f, Enabled = false };
		public byte UnitState
		{
			get { return _UnitState; }
			set
			{
				// Don't do anything if the value is the same
				if (_UnitState == value)
					return;

				// Mark the field as dirty for the network to transmit
				_dirtyFields[0] |= 0x8;
				_UnitState = value;
				hasDirtyFields = true;
			}
		}

		public void SetUnitStateDirty()
		{
			_dirtyFields[0] |= 0x8;
			hasDirtyFields = true;
		}

		private void RunChange_UnitState(ulong timestep)
		{
			if (UnitStateChanged != null) UnitStateChanged(_UnitState, timestep);
			if (fieldAltered != null) fieldAltered("UnitState", _UnitState, timestep);
		}

		protected override void OwnershipChanged()
		{
			base.OwnershipChanged();
			SnapInterpolations();
		}
		
		public void SnapInterpolations()
		{
			PositionInterpolation.current = PositionInterpolation.target;
			RotationInterpolation.current = RotationInterpolation.target;
			HealthInterpolation.current = HealthInterpolation.target;
			UnitStateInterpolation.current = UnitStateInterpolation.target;
		}

		public override int UniqueIdentity { get { return IDENTITY; } }

		protected override BMSByte WritePayload(BMSByte data)
		{
			UnityObjectMapper.Instance.MapBytes(data, _Position);
			UnityObjectMapper.Instance.MapBytes(data, _Rotation);
			UnityObjectMapper.Instance.MapBytes(data, _Health);
			UnityObjectMapper.Instance.MapBytes(data, _UnitState);

			return data;
		}

		protected override void ReadPayload(BMSByte payload, ulong timestep)
		{
			_Position = UnityObjectMapper.Instance.Map<Vector3>(payload);
			PositionInterpolation.current = _Position;
			PositionInterpolation.target = _Position;
			RunChange_Position(timestep);
			_Rotation = UnityObjectMapper.Instance.Map<Quaternion>(payload);
			RotationInterpolation.current = _Rotation;
			RotationInterpolation.target = _Rotation;
			RunChange_Rotation(timestep);
			_Health = UnityObjectMapper.Instance.Map<float>(payload);
			HealthInterpolation.current = _Health;
			HealthInterpolation.target = _Health;
			RunChange_Health(timestep);
			_UnitState = UnityObjectMapper.Instance.Map<byte>(payload);
			UnitStateInterpolation.current = _UnitState;
			UnitStateInterpolation.target = _UnitState;
			RunChange_UnitState(timestep);
		}

		protected override BMSByte SerializeDirtyFields()
		{
			dirtyFieldsData.Clear();
			dirtyFieldsData.Append(_dirtyFields);

			if ((0x1 & _dirtyFields[0]) != 0)
				UnityObjectMapper.Instance.MapBytes(dirtyFieldsData, _Position);
			if ((0x2 & _dirtyFields[0]) != 0)
				UnityObjectMapper.Instance.MapBytes(dirtyFieldsData, _Rotation);
			if ((0x4 & _dirtyFields[0]) != 0)
				UnityObjectMapper.Instance.MapBytes(dirtyFieldsData, _Health);
			if ((0x8 & _dirtyFields[0]) != 0)
				UnityObjectMapper.Instance.MapBytes(dirtyFieldsData, _UnitState);

			// Reset all the dirty fields
			for (int i = 0; i < _dirtyFields.Length; i++)
				_dirtyFields[i] = 0;

			return dirtyFieldsData;
		}

		protected override void ReadDirtyFields(BMSByte data, ulong timestep)
		{
			if (readDirtyFlags == null)
				Initialize();

			Buffer.BlockCopy(data.byteArr, data.StartIndex(), readDirtyFlags, 0, readDirtyFlags.Length);
			data.MoveStartIndex(readDirtyFlags.Length);

			if ((0x1 & readDirtyFlags[0]) != 0)
			{
				if (PositionInterpolation.Enabled)
				{
					PositionInterpolation.target = UnityObjectMapper.Instance.Map<Vector3>(data);
					PositionInterpolation.Timestep = timestep;
				}
				else
				{
					_Position = UnityObjectMapper.Instance.Map<Vector3>(data);
					RunChange_Position(timestep);
				}
			}
			if ((0x2 & readDirtyFlags[0]) != 0)
			{
				if (RotationInterpolation.Enabled)
				{
					RotationInterpolation.target = UnityObjectMapper.Instance.Map<Quaternion>(data);
					RotationInterpolation.Timestep = timestep;
				}
				else
				{
					_Rotation = UnityObjectMapper.Instance.Map<Quaternion>(data);
					RunChange_Rotation(timestep);
				}
			}
			if ((0x4 & readDirtyFlags[0]) != 0)
			{
				if (HealthInterpolation.Enabled)
				{
					HealthInterpolation.target = UnityObjectMapper.Instance.Map<float>(data);
					HealthInterpolation.Timestep = timestep;
				}
				else
				{
					_Health = UnityObjectMapper.Instance.Map<float>(data);
					RunChange_Health(timestep);
				}
			}
			if ((0x8 & readDirtyFlags[0]) != 0)
			{
				if (UnitStateInterpolation.Enabled)
				{
					UnitStateInterpolation.target = UnityObjectMapper.Instance.Map<byte>(data);
					UnitStateInterpolation.Timestep = timestep;
				}
				else
				{
					_UnitState = UnityObjectMapper.Instance.Map<byte>(data);
					RunChange_UnitState(timestep);
				}
			}
		}

		public override void InterpolateUpdate()
		{
			if (IsOwner)
				return;

			if (PositionInterpolation.Enabled && !PositionInterpolation.current.UnityNear(PositionInterpolation.target, 0.0015f))
			{
				_Position = (Vector3)PositionInterpolation.Interpolate();
				//RunChange_Position(PositionInterpolation.Timestep);
			}
			if (RotationInterpolation.Enabled && !RotationInterpolation.current.UnityNear(RotationInterpolation.target, 0.0015f))
			{
				_Rotation = (Quaternion)RotationInterpolation.Interpolate();
				//RunChange_Rotation(RotationInterpolation.Timestep);
			}
			if (HealthInterpolation.Enabled && !HealthInterpolation.current.UnityNear(HealthInterpolation.target, 0.0015f))
			{
				_Health = (float)HealthInterpolation.Interpolate();
				//RunChange_Health(HealthInterpolation.Timestep);
			}
			if (UnitStateInterpolation.Enabled && !UnitStateInterpolation.current.UnityNear(UnitStateInterpolation.target, 0.0015f))
			{
				_UnitState = (byte)UnitStateInterpolation.Interpolate();
				//RunChange_UnitState(UnitStateInterpolation.Timestep);
			}
		}

		private void Initialize()
		{
			if (readDirtyFlags == null)
				readDirtyFlags = new byte[1];

		}

		public UnitNetworkObject() : base() { Initialize(); }
		public UnitNetworkObject(NetWorker networker, INetworkBehavior networkBehavior = null, int createCode = 0, byte[] metadata = null) : base(networker, networkBehavior, createCode, metadata) { Initialize(); }
		public UnitNetworkObject(NetWorker networker, uint serverId, FrameStream frame) : base(networker, serverId, frame) { Initialize(); }

		// DO NOT TOUCH, THIS GETS GENERATED PLEASE EXTEND THIS CLASS IF YOU WISH TO HAVE CUSTOM CODE ADDITIONS
	}
}
