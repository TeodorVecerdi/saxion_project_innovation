using System;
using System.Configuration;
using System.Diagnostics;
using TweetSource.EventSource;
using static Token;
using Debug = UnityEngine.Debug;

public static class TwitterAPI1 {
    public static int DispatchTimeOut { get; set; } = 1;
    private static TweetEventSource source = null;
    private static bool initialized = false;
    private static bool connected = false;

    public static void Initialize(EventHandler<TweetEventArgs> onEventReceived = null, EventHandler<TweetEventArgs> onSourceDown = null, EventHandler<TweetEventArgs> onSourceUp = null) {
        if (initialized) {
            Debug.LogWarning("Trying to initialize when source is already initialized.");
            return;
        }

        source = TweetEventSource.CreateFilterStream();
        if (onEventReceived != null)
            source.EventReceived += onEventReceived;
        if (onEventReceived != null)
            source.SourceDown += onSourceDown;
        if (onEventReceived != null)
            source.SourceUp += onSourceUp;

        var config = source.AuthConfig;
        config.ConsumerKey = CONSUMER_PUBLIC;
        config.ConsumerSecret = CONSUMER_PRIVATE;
        config.Token = API_PUBLIC;
        config.TokenSecret = API_PRIVATE;
        
        Debug.Log("Stream initialized");
        initialized = true;
    }

    public static void Connect() {
        if (!initialized || (connected && source.Active)) {
            Debug.LogWarning("Trying to connect when source is not initialized or is already connected.");
            return;
        }

        try {
            source.Start(new StreamingAPIParameters {Track = new[] {"#indiedev", "#gamedev"}});
            connected = true;
            Debug.Log("Stream connected");
        } catch (Exception ex) {
            Debug.LogError($@"Error reading config: If you're running this for the first time, please make sure you have your version of Twitter.config at application's working directory - {ex.Message}");
            Debug.LogException(ex);
        }
    }

    public static void Dispatch() {
        if (!connected || !source.Active) {
            Debug.LogWarning("Trying to dispatch when source is not active.");
            return;
        }
        
        source.Dispatch(DispatchTimeOut);
    }

    public static void Disconnect() {
        source.Stop();
        source.Cleanup();
        connected = false;
        Debug.Log("Stream disconnected");
    }
}