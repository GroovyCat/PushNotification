using System;
using System.Collections.Generic;
using UnityEngine;
using Firebase;
using Firebase.Messaging;
using Firebase.Extensions;
#if UNITY_ANDROID
using UnityEngine.Android;
using Unity.Notifications.Android; // Unity에서 제공하는 푸시 알림 Package (Version 2.3.2)
#elif UNITY_IOS
using Unity.Notifications.iOS; // Unity에서 제공하는 푸시 알림 Package (Version 2.3.2)
#endif

public class PushNotificationManager : MonoBehaviour
{
    private const string ChannelID = "FirebaseCloudMessagingNotificationChannel"; // 채널 ID 필드
    private int _androidApiLevel; // API 레벨 필드

    void Start()
    {
#if UNITY_ANDROID
        RequestAuthorizationForAndroid();
        InitalizeFirebaseCloudMessaging();
#elif UNITY_IOS
        RequestAuthorizationForApple();
        InitalizeFirebaseCloudMessaging();  
#endif
    }
    
    public void InitalizeFirebaseCloudMessaging()
    {
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task =>
        {
            var dependencyStatus = task.Result;
            if (dependencyStatus == DependencyStatus.Available)
            {
                // Firebase Cloud Meassaging 초기화
                FirebaseMessaging.TokenReceived += OnTokenReceived;
#if UNITY_ANDROID
                FirebaseMessaging.MessageReceived += OnMessageReceivedForAndroid;
#elif UNITY_IOS
                FirebaseMessaging.MessageReceived += OnMessageReceivedForApple;
#endif
                FirebaseMessaging.RequestPermissionAsync().ContinueWithOnMainThread(task =>
                {
                    Debug.Log("push permission: " + task.Status.ToString());
                });
        
                // Token 등록 초기화 
                FirebaseMessaging.TokenRegistrationOnInitEnabled = true;
            }
            else
            {
                Debug.LogError(string.Format(
                    "Could not resolve all Firebase dependencies: {0}",
                    dependencyStatus
                ));
            }
        });
    }

    public void OnTokenReceived(object sender, TokenReceivedEventArgs token)
    {
        Debug.Log("OnTokenReceived: " + token.Token);
    }


#if UNITY_ANDROID
    // 권한 부여 요청 메서드(안드로이드)
    public void RequestAuthorizationForAndroid()
    {
        // Android Version 체크 및 API Level 체크 
        var androidInfo = SystemInfo.operatingSystem;
        Debug.Log("androidInfo: " + androidInfo);

        _androidApiLevel = int.Parse(androidInfo.Substring(androidInfo.IndexOf("-") + 1, 2));
        Debug.Log("apiLevel: " + _androidApiLevel);

        // API 33부터는 알림 게시 권한을 요청해야 함
        if (_androidApiLevel >= 33 &&
            !Permission.HasUserAuthorizedPermission("android.permission.POST_NOTIFICATIONS"))
        {
            Permission.RequestUserPermission("android.permission.POST_NOTIFICATIONS");
        }
        // API 26부터는 채널 ID를 통해 알림 상태를 추적할 수 있음
        if (_androidApiLevel >= 26)
        {
            var channel = new AndroidNotificationChannel()
            {
                Id = ChannelId,
                Name = "PushNotification",
                Importance = Importance.High,
                Description = "Notification",
            };
            AndroidNotificationCenter.RegisterNotificationChannel(channel);
        }
    }

    // 원하는 메세지 작성 가능
    public void OnMessageReceivedForAndroid(object sender, MessageReceivedEventArgs e)
    {
        var type = "";
        var title = "";
        var body = "";

        // 알림 메시지를 위한
        if (e.Message.Notification != null)
        {
            type = "notification";
            title = e.Message.Notification.Title;
            body = e.Message.Notification.Body;
        }
        // 데이터 메시지를 위한
        else if (e.Message.Data.Count > 0)
        {
            type = "data";
            title = e.Message.Data["title"];
            body = e.Message.Data["body"];
        }
        Debug.Log("message type: " + type + ", title: " + title + ", body: " + body);

        var notification = new AndroidNotification()
        {
            SmallIcon = "icon_0",
            Title = title,
	    Text = body,
            FireTime = DateTime.Now    
        };

        if (_androidApiLevel >= 26)
        {
            AndroidNotificationCenter.SendNotification(notification, ChannelId);
        }
        else
        {
            Debug.LogError("Android 8.0 이상의 디바이스에서만 푸시 알림이 정상적으로 표시됩니다.");
        }
    }
#elif UNITY_IOS
    // 권한 부여 요청하는 메서드(iOS)
    public IEnumerator<string> RequestAuthorizationForApple()
    {
        var request = new AuthorizationRequest(AuthorizationOption.Alert | AuthorizationOption.Badge, true);
        while (!request.IsFinished)
        {
            yield return null;
        }
    }

    // 원하는 메세지 작성 가능
    public void OnMessageReceivedForApple(object sender, MessageReceivedEventArgs e)
    {
        var type = "";
        var title = "";
        var body = "";
        const int fireTimeinSeconds = 0;

        var timeTrigger = new iOSNotificationTimeIntervalTrigger()
        {
            TimeInterval = new TimeSpan(0, 0, fireTimeinSeconds),
            Repeats = false
        };

        // 알림 메시지를 위한
        if (e.Message.Notification != null)
        {
            type = "notification";
            title = e.Message.Notification.Title;
            body = e.Message.Notification.Body;
        }
        // 데이터 메시지를 위한
        else if (e.Message.Data.Count > 0)
        {
            type = "data";
            title = e.Message.Data["title"];
            body = e.Message.Data["body"];
        }
        Debug.Log("message type: " + type + ", title: " + title + ", body: " + body);

        var notification = new iOSNotification()
        {
            Identifier = "_notification_01",
            Title = title,
            Body = body,
            Subtitle = "",
            ShowInForeground = true,
            ForegroundPresentationOption = (PresentationOption.Alert | PresentationOption.Sound),
            CategoryIdentifier = "category_a",
            ThreadIdentifier = "thread1",
            Trigger = timeTrigger,
        };

        iOSNotificationCenter.ScheduleNotification(notification);
    }
#endif
}
