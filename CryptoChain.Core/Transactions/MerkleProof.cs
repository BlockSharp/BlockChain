using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using CryptoChain.Core.Helpers;

namespace CryptoChain.Core.Transactions
{
    public class MerkleProof : Queue<(byte[] hash, bool side)>
    {
        private static JsonSerializerOptions _options = new() {Converters = {new MerkleProofQueueConverter()}};
            
        public MerkleProof(){}
        public MerkleProof(Queue<(byte[], bool)> proof) : base(proof) { }
        public MerkleProof(List<(byte[], bool)> proof) : base(new Queue<(byte[] hash, bool side)>(proof)) {}

        public MerkleProof Clone()
            => new MerkleProof(this.ToList());

        public override string ToString()
            => JsonSerializer.Serialize(this, _options);

        public MerkleProof(string serialized) : base(JsonSerializer.Deserialize<MerkleProof>(serialized, _options) ?? throw new InvalidOperationException()) {}
    }
}