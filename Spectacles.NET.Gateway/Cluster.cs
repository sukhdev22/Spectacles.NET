// ReSharper disable MemberCanBePrivate.Global

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Spectacles.NET.Gateway.EventArgs;
using Spectacles.NET.Util.Logging;

namespace Spectacles.NET.Gateway
{
	/// <inheritdoc />
	/// <summary>
	///     A Cluster of Shards
	/// </summary>
	public class Cluster : IDisposable
	{
		/// <summary>
		///     Creates a new instance and uses either provided or recommended shard count.
		/// </summary>
		/// <param name="token">The token of the bot.</param>
		/// <param name="shardCount">The shard count to use.</param>
		/// <param name="options">Connection Options this cluster should use</param>
		public Cluster(string token, int? shardCount = null, ConnectionOptions options = null)
		{
			options ??= new ConnectionOptions();
			Gateway = Util.GetGateway(token, shardCount, options.ShardingSystem);
			ConnectionOptions = options;
		}

		/// <summary>
		/// 	Creates a new instance which spawns a range of Shards from id.
		/// </summary>
		/// <param name="token">The token of the bot.</param>
		/// <param name="shardCount">The shard count to use.</param>
		/// <param name="shardIds">The ids of shards to spawn.</param>
		/// <param name="options">Connection Options this cluster should use</param>
		public Cluster(string token, int shardCount, IEnumerable<int> shardIds, ConnectionOptions options = null) : this(token, shardCount, options)
			=> ShardIds = shardIds;

		/// <summary>
		///     A Dictionary of Shards mapped to there Id.
		/// </summary>
		public Dictionary<int, Shard> Shards { get; } = new Dictionary<int, Shard>();

		/// <summary>
		///     The Gateway of this Cluster
		/// </summary>
		public IGateway Gateway { get; }
		
		/// <summary>
		/// The ShardIds this Cluster will spawn
		/// </summary>
		public IEnumerable<int> ShardIds { get; }
		
		/// <summary>
		/// The Connection Options this Cluster should use for each shard
		/// </summary>
		public ConnectionOptions ConnectionOptions { get; }

		/// <summary>
		///     The ShardCount provided by the Constructor.
		/// </summary>
		private int ShardCount
			=> Gateway.ShardCount;

		/// <summary>
		///     If this Cluster is already disposed.
		/// </summary>
		private bool Disposed { get; set; }

		/// <inheritdoc />
		/// <summary>
		///     Disposes this Cluster.
		/// </summary>
		public void Dispose()
		{
			if (Disposed) return;
			foreach (var shard in Shards.Values) shard.Dispose();
			Disposed = true;
		}

		/// <summary>
		///     Event emitted when Logs are received.
		/// </summary>
		public event EventHandler<LogEventArgs> Log;

		/// <summary>
		///     Event emitted when Shards fails to Connect with an unrecoverable code.
		/// </summary>
		public event EventHandler<ExceptionEventArgs> Error;

		/// <summary>
		///     Event emitted when Shards receive Dispatches.
		/// </summary>
		public event EventHandler<DispatchEventArgs> Dispatch;

		/// <summary>
		///     Event emitted when Shards send packets.
		/// </summary>
		public event EventHandler<SendEventArgs> Send;
		
		/// <summary>
		/// 	Event emitted when Shards Heartbeat changes, Latency in ms.
		/// </summary>
		public event EventHandler<LatencyUpdateArgs> LatencyUpdate;

		/// <summary>
		///     Connects all Shards of this Cluster to the Gateway.
		/// </summary>
		/// <returns>Task</returns>
		public async Task ConnectAsync()
		{
			if (!Gateway.Ready) await Gateway.InitializeAsync();

			if (ShardIds != null)
				foreach (var shardId in ShardIds)
					Shards.Add(shardId, new Shard(this, shardId, ConnectionOptions));
			else
				for (var i = 0; i < ShardCount; i++)
					Shards.Add(i, new Shard(this, i, ConnectionOptions));

			_log(LogLevel.INFO, $"Spawning {Shards.Count} shard(s)");

			foreach (var shard in Shards.Values)
			{
				shard.Log += Log;
				shard.Error += (sender, e) => Error?.Invoke(sender, new ExceptionEventArgs(shard.Id, e));
				shard.Dispatch += Dispatch;
				shard.Send += Send;
				shard.LatencyUpdate += LatencyUpdate;
				await shard.ConnectAsync();
			}

			_log(LogLevel.INFO, "Finished spawning shards");
		}

		/// <summary>
		///     Emits something on the Log event
		/// </summary>
		/// <param name="level">The LogLevel of this log</param>
		/// <param name="message">The message of this log</param>
		private void _log(LogLevel level, string message)
			=> Log?.Invoke(this, new LogEventArgs(level, "Cluster", message));
	}
}
