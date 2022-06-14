using System.Collections.Generic;
using System.Linq;
using Save;
using Terrain;
using UnityEngine;

namespace Players
{
    public class PlayerController : MonoBehaviour
    {
        [SerializeField] private float speed = 6;
        
        private Rigidbody2D _rigidbody2D;
        private TerrainGenerator _terrainGenerator;
        private Vector3Int _prevPos;
        private List<LayerSave> _layers;
        
        private void Start()
        {
            _rigidbody2D = GetComponent<Rigidbody2D>();
            _terrainGenerator = FindObjectOfType<TerrainGenerator>();
            _prevPos = _terrainGenerator.tilemap.WorldToCell(_rigidbody2D.position);
            _layers = SaveManager.Instance.CurrentSave.layers;

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
            var newPos = _terrainGenerator.tilemap.WorldToCell(_rigidbody2D.position);
            
            var oldChunkPos = Chunk.ToChunkPos(_prevPos);
            var newChunkPos = Chunk.ToChunkPos(newPos);
            
            if (oldChunkPos != newChunkPos)
            {
                EnteredNewChunk(oldChunkPos, newChunkPos);
            }
            
            _prevPos = _terrainGenerator.tilemap.WorldToCell(_rigidbody2D.position);
        }
        
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!other.gameObject.CompareTag("BaseEnvironment")) return;
            speed /= 1.5f;
        }
        
        private void OnTriggerExit2D(Collider2D other)
        {
            if (!other.gameObject.CompareTag("BaseEnvironment")) return;
            speed *= 1.5f;
        }

        private void EnteredNewChunk(Vector2Int oldChunkPos, Vector2Int newChunkPos)
        {
            var oldPoses = new List<Vector2Int>();
            var newPoses = new List<Vector2Int>();
            
            for (var i = -2; i < 2; i++)
            {
                for (var j = -2; j < 2; j++)
                {
                    var offset = new Vector2Int(i, j);
                    oldPoses.Add(oldChunkPos + offset);
                    newPoses.Add(newChunkPos + offset);
                }
            }

            oldPoses.RemoveAll(pos => newPoses.Contains(pos));

            if (oldPoses.Any())
            {
                foreach (var layer in _layers)
                {
                    layer.UnloadChunks(oldPoses);
                }
            }

            foreach (var n in newPoses)
            {
                foreach (var layer in _layers)
                {
                    layer.LoadOrCreateChunk(n);
                }
            }
        }
    }
}
