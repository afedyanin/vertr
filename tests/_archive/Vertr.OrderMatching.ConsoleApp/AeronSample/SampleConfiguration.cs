using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Adaptive.Aeron;

namespace Vertr.OrderMatching.ConsoleApp.AeronSample;

internal class SampleConfiguration
{
    private const string ChannelProp = "aeron.sample.channel";
    private const string StreamIDProp = "aeron.sample.streamId";

    private const string PingChannelProp = "aeron.sample.ping.channel";
    private const string PongChannelProp = "aeron.sample.pong.channel";
    private const string PingStreamIDProp = "aeron.sample.ping.streamId";
    private const string PongStreamIDProp = "aeron.sample.pong.streamId";
    private const string WarmupNumberOfMessagesProp = "aeron.sample.warmup.messages";
    private const string WarmupNumberOfIterationsProp = "aeron.sample.warmup.iterations";
    private const string RandomMessageLengthProp = "aeron.sample.randomMessageLength";

    private const string FrameCountLimitProp = "aeron.sample.frameCountLimit";
    private const string MessageLengthProp = "aeron.sample.messageLength";
    private const string NumberOfMessagesProp = "aeron.sample.messages";
    private const string LingerTimeoutMsProp = "aeron.sample.lingerTimeout";

    public static readonly string CHANNEL;
    public static readonly string PING_CHANNEL;
    public static readonly string PONG_CHANNEL;
    public static readonly int STREAM_ID;
    public static readonly int PING_STREAM_ID;
    public static readonly int PONG_STREAM_ID;
    public static readonly bool RANDOM_MESSAGE_LENGTH;
    public static readonly int FRAGMENT_COUNT_LIMIT;
    public static readonly int MESSAGE_LENGTH;
    public static readonly int NUMBER_OF_MESSAGES;
    public static readonly int WARMUP_NUMBER_OF_MESSAGES;
    public static readonly int WARMUP_NUMBER_OF_ITERATIONS;
    public static readonly long LINGER_TIMEOUT_MS;

    static SampleConfiguration()
    {
        CHANNEL = Config.GetProperty(ChannelProp, "aeron:udp?endpoint=localhost:40123");
        STREAM_ID = Config.GetInteger(StreamIDProp, 10);
        PING_CHANNEL = Config.GetProperty(PingChannelProp, "aeron:udp?endpoint=localhost:40123");
        PONG_CHANNEL = Config.GetProperty(PongChannelProp, "aeron:udp?endpoint=localhost:40124");
        PING_STREAM_ID = Config.GetInteger(PingStreamIDProp, 10);
        PONG_STREAM_ID = Config.GetInteger(PongStreamIDProp, 10);
        FRAGMENT_COUNT_LIMIT = Config.GetInteger(FrameCountLimitProp, 256);
        MESSAGE_LENGTH = Config.GetInteger(MessageLengthProp, 32);
        RANDOM_MESSAGE_LENGTH = Config.GetBoolean(RandomMessageLengthProp);
        NUMBER_OF_MESSAGES = Config.GetInteger(NumberOfMessagesProp, 1000000);
        WARMUP_NUMBER_OF_MESSAGES = Config.GetInteger(WarmupNumberOfMessagesProp, 10000);
        WARMUP_NUMBER_OF_ITERATIONS = Config.GetInteger(WarmupNumberOfIterationsProp, 5);
        LINGER_TIMEOUT_MS = Config.GetLong(LingerTimeoutMsProp, 5000);
    }
}
