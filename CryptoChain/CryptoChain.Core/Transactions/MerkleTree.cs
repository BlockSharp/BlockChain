using System;
using System.Collections.Generic;
using System.Linq;
using CryptoChain.Core.Cryptography.Hashing;

namespace CryptoChain.Core.Transactions
{
    /// <summary>
    /// A tree structure for the transaction ids
    /// </summary>
    public class MerkleTree
    { 
        public Node Root { get; set; }
        public byte[] MerkleRoot => Root.Hash;

        /// <summary>
        /// Create a new merkleTree from a list of transactions
        /// </summary>
        /// <param name="transactions">A list of transactions</param>
        public MerkleTree(TransactionList transactions) : this(transactions.Select(x => x.TxId)){}
        
        /// <summary>
        /// Create a new merkleTree from a list of transaction ids/hashes
        /// </summary>
        /// <param name="txIds">The list of tx ids</param>
        public MerkleTree(IEnumerable<byte[]> txIds)
            => Root = BuildTree(new Queue<Node>(txIds.Select(x => new Node(x))));

        /// <summary>
        /// Create a new empty merkleTree, used for thin clients to use the ValidateInclusionProof function
        /// </summary>
        /// <param name="merkleRoot">The root value</param>
        public MerkleTree(byte[] merkleRoot)
            => Root = new Node(merkleRoot);
        
        /// <summary>
        /// Create a new merkleTree
        /// </summary>
        /// <param name="rootNode">The root node</param>
        public MerkleTree(Node rootNode)
            => Root = rootNode;

        /// <summary>
        /// Build the merkle tree
        /// </summary>
        /// <param name="items">All leaf nodes</param>
        /// <returns>A root node containing a tree</returns>
        private Node BuildTree(Queue<Node> items)
        {
            if (items.Count == 1)
                return items.Dequeue();

            var nodes = new Queue<Node>();
            
            while (items.Any())
            {
                //If there are no other anymore than it uses itself twice
                var first = items.Count >= 2 ? items.Dequeue() : items.Peek();
                var second = items.Dequeue();
                var both = new Node(first, second);
                nodes.Enqueue(both);
            }
            
            return BuildTree(nodes);
        }

        /// <summary>
        /// Get proof, that means all siblings needed to verify a hash from the leaves to the top (all siblings)
        /// The bool indicates if it is the left (0) or right (1) pair in the tree
        /// A proof like this can be sent to a thin client
        /// </summary>
        /// <param name="index">The index of the leaf you want to generate a proof for</param>
        /// <returns>A Queue with (hash, side): the siblings along the path to the leaf starting from the leaf</returns>
        public MerkleProof GetInclusionProof(int index)
        {
            int depth = (int)Math.Ceiling(Math.Log2(Root.LeafCount));
            Queue<bool> path = new (Convert.ToString(index, 2)
                .PadLeft(depth, '0').Select(x => x == '1'));

            var proof = new List<(byte[], bool)>();

            //Follow path, get siblings of every step in the path and the sibling of the desired node
            var node = Root;
            while (path.Any())
            {
                bool next = path.Dequeue();
                //Get sibling, needed for the proof
                proof.Add((!next ? node.Right.Hash : node.Left.Hash, !next));
                node = node.Navigate(next);
            }

            proof.Reverse();
            return new MerkleProof(proof);
        }

        /// <summary>
        /// Validate a inclusion proof
        /// </summary>
        /// <param name="hash">The hash of the leaf to be verified</param>
        /// <param name="p">The siblings along the path from the leaf to the top</param>
        /// <returns>true if the proof succeeds (and the result equals the merkleRoot)</returns>
        public bool ValidateInclusionProof(byte[] hash, MerkleProof p)
        {
            var proof = p.Clone();
            while (proof.Any())
            {
                var next = proof.Dequeue();
                byte[] buffer = new byte[hash.Length + next.hash.Length];

                next.hash.CopyTo(buffer, next.side ? next.hash.Length : 0);
                hash.CopyTo(buffer, next.side ? 0 : next.hash.Length);
                
                hash = Hash.HASH_256(buffer);
            }

            return hash.SequenceEqual(MerkleRoot);
        }

        public class Node
        {
            public byte[] Hash { get; set; }
            
            public Node? Left { get; set; }
            public Node? Right { get; set; }

            public bool IsLeaf => Left == null && Right == null;
            public int LeafCount => IsLeaf ? 1 : (Left?.LeafCount ?? 0) + (Right?.LeafCount ?? 0);
            public int Count => 1 + (Left?.Count ?? 0) + (Right?.Count ?? 0);
            
            /// <summary>
            /// Create a new node
            /// </summary>
            /// <param name="hash">The value the node contains</param>
            public Node(byte[] hash)
                => Hash = hash;

            /// <summary>
            /// Navigate through the tree
            /// </summary>
            /// <param name="direction">When 0: go left, when 1: go right</param>
            /// <returns>The node where is navigated to</returns>
            public Node? Navigate(bool direction)
                => direction ? Right : Left;

            /// <summary>
            /// Create a new node
            /// </summary>
            /// <param name="left">The left child</param>
            /// <param name="right">The right child</param>
            public Node(Node left, Node right)
            {
                Left = left;
                Right = right;
                byte[] both = new byte[Left.Hash.Length + Right.Hash.Length];
                Left.Hash.CopyTo(both, 0);
                Right.Hash.CopyTo(both, Left.Hash.Length);
                Hash = Cryptography.Hashing.Hash.HASH_256(both);
            }
        }
    }
}