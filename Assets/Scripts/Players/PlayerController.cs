using Terrain;
using UnityEngine;

namespace Players
{
    public class PlayerController : MonoBehaviour
    {
        [SerializeField] private float speed = 6;
        
        private Rigidbody2D _rigidbody2D;
        private TerrainManager _terrainManager;
        private Vector3Int _prevPos;
        
        private void Start()
        {
            _rigidbody2D = GetComponent<Rigidbody2D>();
            _terrainManager = FindObjectOfType<TerrainManager>();
            _prevPos = _terrainManager.tilemap.WorldToCell(_rigidbody2D.position);

            var startChunk = Chunk.ToChunkPos(_prevPos);
            EnteredNewChunk(startChunk, startChunk);
        }

        private void FixedUpdate()
        {
            var dir = Vector2.zero;
            
            if (Input.GetKey(KeyCode.W))
            {
                dir += Vector2.up;
            }

            if (Input.GetKey(KeyCode.A))
            {
                dir += Vector2.left;
            }
            
            if (Input.GetKey(KeyCode.S))
            {
                dir += Vector2.down;
            }
            
            if (Input.GetKey(KeyCode.D))
            {
                dir += Vector2.right;
            }

            if (dir == Vector2.zero) return;
            
            dir.Normalize();
            
            _rigidbody2D.MovePosition(_rigidbody2D.position + dir * (Time.deltaTime * speed));
            var newPos = _terrainManager.tilemap.WorldToCell(_rigidbody2D.position);
            
            var oldChunkPos = Chunk.ToChunkPos(_prevPos);
            var newChunkPos = Chunk.ToChunkPos(newPos);

            Debug.Log("Old Pos" + _prevPos);
            Debug.Log("New Pos" + newPos);
            
            if (oldChunkPos != newChunkPos)
            {
                EnteredNewChunk(oldChunkPos, newChunkPos);
            }
            
            _prevPos = _terrainManager.tilemap.WorldToCell(_rigidbody2D.position);
        }

        private void EnteredNewChunk(Vector2Int oldChunkPos, Vector2Int newChunkPos)
        {
            _terrainManager.Save.UnloadChunks(oldChunkPos);
            _terrainManager.Save.LoadOrCreateChunk(newChunkPos);
        }
    }
}
