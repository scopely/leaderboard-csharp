using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BookSleeve;

namespace Leaderboard.Redis
{
    public interface IRedisConnectionManager : IDisposable
    {
        RedisConnection GetConnection();
        RedisConnection GetConnection(bool waitOnOpen);
        void Reset();
        void Reset(bool abort);
    }
}
