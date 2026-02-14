using Labb3.Converters;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;

namespace Labb3.Models
{
    public class QuestionPack
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public string Name { get; set; }
        public Difficulty Difficulty { get; set; }
        public int TimeLimitInSeconds { get; set; }
        public string CategoryName { get; set; }
        public List<Question> Questions { get; set; }

        public QuestionPack(string name = "<PackName>", Difficulty difficulty = Difficulty.Medium, int timeLimitInSeconds = 30)
        {
            Name = name;
            Difficulty = difficulty;
            TimeLimitInSeconds = timeLimitInSeconds;
            Questions = new List<Question>();
        }
    }
}