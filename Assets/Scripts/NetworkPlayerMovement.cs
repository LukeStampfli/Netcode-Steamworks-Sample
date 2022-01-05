using Unity.Netcode;
using UnityEngine;
using UnityEngine.Assertions;

public class NetworkPlayerMovement : NetworkBehaviour
{
    // only used on the server to cache inputs from RPC
    private byte m_ServerInput;

    [SerializeField]
    private float m_Speed = 5;

    private void FixedUpdate()
    {
        if (IsOwner)
        {
            int input = 0;
            input |= Input.GetKey(KeyCode.W) ? 1 << 0 : 0;
            input |= Input.GetKey(KeyCode.A) ? 1 << 1 : 0;
            input |= Input.GetKey(KeyCode.S) ? 1 << 2 : 0;
            input |= Input.GetKey(KeyCode.D) ? 1 << 3 : 0;
            PlayerInputServerRpc((byte) input);
        }

        if (IsServer)
        {
            bool w = (m_ServerInput & 1 << 0) != 0;
            bool a = (m_ServerInput & 1 << 1) != 0;
            bool s = (m_ServerInput & 1 << 2) != 0;
            bool d = (m_ServerInput & 1 << 3) != 0;

            var direction = Vector3.zero;
            if (w) direction += Vector3.forward;
            if (a) direction += Vector3.left;
            if (s) direction += Vector3.back;
            if (d) direction += Vector3.right;

            if (direction.sqrMagnitude > 0)
            {
                direction.Normalize();
            }

            transform.position += direction * m_Speed * Time.fixedDeltaTime;
        }
    }

    [ServerRpc]
    private void PlayerInputServerRpc(byte input)
    {
        m_ServerInput = input;
    }
}
