using System;
using UnityEngine;
using Firebase;
using Firebase.Messaging;
using Firebase.Extensions;
#if UNITY_ANDROID
using UnityEngine.Android; 
using Unity.Notifications.Android; // Unity에서 제공하는 푸시 알림 Package (Version 2.3.2)
#endif
#if UNITY_IOS
using Unity.Notifications.iOS; // Unity에서 제공하는 푸시 알림 Package (Version 2.3.2)
using System.Collections.Generic;
#endif



public class PushNotificationManager : MonoBehaviour
{
    readonly private string _channelID = "FirebaseCloudMessagingTestChannel"; // 채널 ID 필드
    private int _apiLevel; // API 레벨 필드

    void Start()
    {
#if UNITY_ANDROID
        RequestAuthorizationForAndroid();
        OnFirebaseCloudMessagingForAndroid();
#endif
#if UNITY_IOS
		RequestAuthorization();
        OnFirebaseCloudMessagingForApple();  
#endif
    }

#if UNITY_ANDROID
    // 권한 부여 요청 메서드(안드로이드)
    public void RequestAuthorizationForAndroid()
    {
        // Android Version 체크 및 API Level 체크 
        string androidInfo = SystemInfo.operatingSystem;
        Debug.Log("androidInfo: " + androidInfo);

        _apiLevel = int.Parse(androidInfo.Substring(androidInfo.IndexOf("-") + 1, 2));
        Debug.Log("apiLevel: " + _apiLevel);

        // API 33부터는 알림 게시 권한을 요청해야 함
        if (_apiLevel >= 33 &&
            !Permission.HasUserAuthorizedPermission("android.permission.POST_NOTIFICATIONS"))
        {
            Permission.RequestUserPermission("android.permission.POST_NOTIFICATIONS");
        }
        // API 26부터는 채널 ID를 통해 알림 상태를 추적할 수 있음
        if (_apiLevel >= 26)
        {
            var channel = new AndroidNotificationChannel()
            {
                Id = _channelID,
                Name = "test",
                Importance = Importance.High,
                Description = "for test",
            };
            AndroidNotificationCenter.RegisterNotificationChannel(channel);
        }
    }

    public void OnFirebaseCloudMessagingForAndroid()
    {
        // Dependecy(Google Play 버전) 체크 및 Firebase Cloud Messaging 실행 메서드
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task =>
        {
            var dependencyStatus = task.Result;
            if (dependencyStatus == DependencyStatus.Available)
            {
                Debug.Log("Google Play version OK");

                // Firebase Cloud Meassaging 초기화
                FirebaseMessaging.TokenReceived += OnTokenReceivedForAndroid;
                FirebaseMessaging.MessageReceived += OnMessageReceivedForAndroid;
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

    public void OnTokenReceivedForAndroid(object sender, TokenReceivedEventArgs token)
    {
        Debug.Log("OnTokenReceived: " + token.Token);
    }

    // 원하는 메세지 작성 가능
    public void OnMessageReceivedForAndroid(object sender, MessageReceivedEventArgs e)
    {
        string type = "";
        string title = "";
        string body = "";

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

        var notification = new AndroidNotification();
        {
            notification.SmallIcon = "icon_0";
            notification.Title = title;
            notification.Text = body;
            notification.FireTime = DateTime.Now;
        }

        if (_apiLevel >= 26)
        {
            AndroidNotificationCenter.SendNotification(notification, _channelID);
        }
        else
        {
            Debug.LogError("Android 8.0 이상의 디바이스에서만 푸시 알림이 정상적으로 표시됩니다.");
        }
    }
#endif


#if UNITY_IOS
    // 권한 부여 요청하는 메서드(iOS)
    public IEnumerator<string> RequestAuthorization()
    {
        var request = new AuthorizationRequest(AuthorizationOption.Alert | AuthorizationOption.Badge, true);
        while (!request.IsFinished)
        {
            yield return null;
        }

        string respond = "\n RequestAuthorization: ";
        respond += "\n finished: " + request.IsFinished;
        respond += "\n granted :  " + request.Granted;
        respond += "\n error:  " + request.Error;
        respond += "\n deviceToken:  " + request.DeviceToken;
        Debug.Log(respond);
    }

    public void OnFirebaseCloudMessagingForApple()
    {
        // Dependecy 체크 및 FirebaseCloudMessaging 실행 메서드
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task =>
        {
            var dependencyStatus = task.Result;
            if (dependencyStatus == DependencyStatus.Available)
            {
                // Firebase Cloud Meassaging 초기화
                FirebaseMessaging.TokenReceived += OnTokenReceivedForApple;
                FirebaseMessaging.MessageReceived += OnMessageReceivedForApple;
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

    public void OnTokenReceivedForApple(object sender, TokenReceivedEventArgs token)
    {
        Debug.Log("ontokenreceived: " + token.Token);
    }

    // 원하는 메세지 작성 가능
    public void OnMessageReceivedForApple(object sender, MessageReceivedEventArgs e)
    {
        string type = "";
        string title = "";
        string body = "";
        int fireTimeinSeconds = 1;

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
            Title = "Title",
            Body = "Scheduled at: " + DateTime.Now.ToShortDateString() + " triggered in 5 seconds",
            Subtitle = "This is a subtitle, something, something important...",
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
