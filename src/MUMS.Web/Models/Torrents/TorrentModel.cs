using System;
using UTorrentAPI;
using System.Runtime.Serialization;
using System.Diagnostics;

namespace MUMS.Web.Models
{
    /// <summary>
    /// Represents a Torrent file definition in uTorrent.
    /// All properties have been supplied with a remarks-tag that notes the definition of the fields
    /// found on http://www.utorrent.com/developers/webapi.
    /// </summary>
    [DebuggerDisplay("Name = {Name}")]
    [DataContract]
    public class TorrentModel
    {
        /// <summary>
        /// Unique torrent hash.
        /// <remarks>0	HASH (string)</remarks>
        /// </summary>
        [DataMember]
        public string Hash { get; private set; }

        /// <summary>
        /// A bitfield of integers.
        /// <remarks>1	STATUS (integer) { cast to an enum for this property }</remarks>
        /// </summary>
        [DataMember]
        public TorrentStatus Status { get; private set; }

        /// <summary>
        /// Name of the torrent.
        /// <remarks>2	NAME (string)</remarks>
        /// </summary>
        [DataMember]
        public string Name { get; private set; }

        /// <summary>
        /// The total size of the torrent in bytes.
        /// <remarks>3	SIZE (integer in bytes)</remarks>
        /// </summary>
        [DataMember]
        public long SizeInBytes { get; private set; }

        /// <summary>
        /// The progress of the download in percentage, 0.00 - 100.00
        /// <remarks>4	PERCENT PROGRESS (integer in per mils) { divided by 10 for this property }</remarks>
        /// </summary>
        [DataMember]
        public double Percentage { get; private set; }

        /// <summary>
        /// The number of bytes that have been downloaded so far.
        /// <remarks>5	DOWNLOADED (integer in bytes)</remarks>
        /// </summary>
        [DataMember]
        public long DownloadedInBytes { get; private set; }

        /// <summary>
        /// The number of bytes that have been uploaded so far.
        /// <remarks>6	UPLOADED (integer in bytes)</remarks>
        /// </summary>
        [DataMember]
        public long UploadedInBytes { get; private set; }

        /// <summary>
        /// The uploaded/downloaded ratio of the torrent.
        /// <remarks>7	RATIO (integer in per mils) { divided by 10 for this property }</remarks>
        /// </summary>
        [DataMember]
        public double Ratio { get; private set; }

        /// <summary>
        /// The upload speed in bytes/sec.
        /// <remarks>8	UPLOAD SPEED (integer in bytes per second)</remarks>
        /// </summary>
        [DataMember]
        public long UploadSpeedInBytes { get; private set; }

        /// <summary>
        /// The download speed in bytes/sec.
        /// <remarks>9	DOWNLOAD SPEED (integer in bytes per second)</remarks>
        /// </summary>
        [DataMember]
        public long DownloadSpeedInBytes { get; private set; }

        /// <summary>
        /// The estimated time until a download is finished. Wrapped in a TimeSpan
        /// for handy abstraction.
        /// <remarks>10	ETA (integer in seconds)</remarks>
        /// </summary>
        [DataMember]
        public TimeSpan EstimatedTime { get; private set; }

        /// <summary>
        /// The estimated time (in full seconds) until a download is finished.
        /// <remarks>10	ETA (integer in seconds)</remarks>
        /// </summary>
        [DataMember]
        public int EstimatedTimeSeconds { get; private set; }

        /// <summary>
        /// The label that the torrent has been associated with.
        /// <remarks>11	LABEL (string)</remarks>
        /// </summary>
        [DataMember(EmitDefaultValue = true)]
        public string Label { get; private set; }

        /// <summary>
        /// The number of peers that are connected.
        /// <remarks>12	PEERS CONNECTED (integer)</remarks>
        /// </summary>
        [DataMember]
        public int PeersConnected { get; private set; }

        /// <summary>
        /// The number of peers available in the swarm.
        /// <remarks>13	PEERS IN SWARM (integer)</remarks>
        /// </summary>
        [DataMember]
        public int PeersInSwarm { get; private set; }

        /// <summary>
        /// The number of seeds that are connected.
        /// <remarks>14	SEEDS CONNECTED (integer)</remarks>
        /// </summary>
        [DataMember]
        public int SeedsConnected { get; private set; }

        /// <summary>
        /// The number of seeds available in the swarm.
        /// <remarks>15	SEEDS IN SWARM (integer)</remarks>
        /// </summary>
        [DataMember]
        public int SeedsInSwarm { get; private set; }

        /// <summary>
        /// The number of unique copies of the file that are available between yourself and the peers you're connected to.
        /// <remarks>16	AVAILABILITY (integer in 1/65535ths)</remarks>
        /// </summary>
        [DataMember]
        public long Availability { get; private set; }

        /// <summary>
        /// The queue order assigned in uTorrent.
        /// <remarks>17	TORRENT QUEUE ORDER (integer)</remarks>
        /// </summary>
        [DataMember]
        public int QueueOrder { get; private set; }

        /// <summary>
        /// The number of bytes remaining to be downloaded.
        /// <remarks>18	REMAINING (integer in bytes)</remarks>
        /// </summary>
        [DataMember]
        public long RemainingInBytes { get; private set; }

        public TorrentModel()
        {
        }

        public TorrentModel(Torrent fromTorrent)
        {
            this.Availability = fromTorrent.Availability;
            this.DownloadedInBytes = fromTorrent.DownloadedBytes;
            this.DownloadSpeedInBytes = fromTorrent.DownloadBytesPerSec;
            this.EstimatedTime = TimeSpan.FromSeconds(fromTorrent.EtaInSecs);
            this.EstimatedTimeSeconds = fromTorrent.EtaInSecs;
            this.Hash = fromTorrent.Hash;
            this.Label = fromTorrent.Label;
            this.Name = fromTorrent.Name;
            this.PeersConnected = fromTorrent.PeersConnected;
            this.PeersInSwarm = fromTorrent.PeersInSwarm;
            this.Percentage = fromTorrent.ProgressInMils / 10d;
            this.QueueOrder = fromTorrent.QueueOrder;
            this.Ratio = fromTorrent.RatioInMils / 1000d;
            this.RemainingInBytes = fromTorrent.RemainingBytes;
            this.SeedsConnected = fromTorrent.SeedsConnected;
            this.SeedsInSwarm = fromTorrent.SeedsInSwarm;
            this.SizeInBytes = fromTorrent.SizeInBytes;
            this.Status = fromTorrent.Status;
            this.UploadedInBytes = fromTorrent.UploadedBytes;
            this.UploadSpeedInBytes = fromTorrent.UploadBytesPerSec;
        }

        /// <summary>
        /// Constructor that takes an array of strings that are used to populate the Torrent properties.
        /// The properties must be supplied in the specific order defined at http://www.utorrent.com/developers/webapi
        /// </summary>
        /// <param name="input">An list of strings that are ordered as defined at http://www.utorrent.com/developers/webapi </param>
        public TorrentModel(string[] input)
        {
            int index = 0;

            // 0	HASH (string)
            Hash = input[index++];

            // 1	STATUS* (integer)
            Status = (TorrentStatus)int.Parse(input[index++]);

            // 2	NAME (string)
            Name = input[index++];

            // 3	SIZE (integer in bytes)
            SizeInBytes = long.Parse(input[index++]);

            // 4	PERCENT PROGRESS (integer in per mils)
            Percentage = double.Parse(input[index++]) / 10d;

            // 5	DOWNLOADED (integer in bytes)
            DownloadedInBytes = long.Parse(input[index++]);

            // 6	UPLOADED (integer in bytes)
            UploadedInBytes = long.Parse(input[index++]);

            // 7	RATIO (integer in per mils)
            Ratio = double.Parse(input[index++]) / 1000d;

            // 8	UPLOAD SPEED (integer in bytes per second)
            UploadSpeedInBytes = long.Parse(input[index++]);

            // 9	DOWNLOAD SPEED (integer in bytes per second)
            DownloadSpeedInBytes = long.Parse(input[index++]);

            // 10	ETA (integer in seconds)
            EstimatedTime = TimeSpan.FromSeconds(double.Parse(input[index++]));
            EstimatedTimeSeconds = (int)EstimatedTime.TotalSeconds;

            // 11	LABEL (string)
            Label = input[index++];

            // 12	PEERS CONNECTED (integer),	
            PeersConnected = int.Parse(input[index++]);

            // 13	PEERS IN SWARM (integer),	
            PeersInSwarm = int.Parse(input[index++]);

            // 14	SEEDS CONNECTED (integer),	
            SeedsConnected = int.Parse(input[index++]);

            // 15	SEEDS IN SWARM (integer),	
            SeedsInSwarm = int.Parse(input[index++]);

            // 16	AVAILABILITY (integer in 1/65535ths),	
            Availability = long.Parse(input[index++]);

            // 17	TORRENT QUEUE ORDER (integer),	
            QueueOrder = int.Parse(input[index++]);

            // 18	REMAINING (integer in bytes)
            RemainingInBytes = long.Parse(input[index++]);
        }
    }
}