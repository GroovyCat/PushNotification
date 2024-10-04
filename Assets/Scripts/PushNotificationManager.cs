using Firebase.Messaging;
using Firebase;
using System.Collections.Generic;
using UnityEngine;
using Firebase.Extensions;
// using UnityEngine.Android;
using Unity.Notifications.iOS;
// using Unity.Notifications.Android;
using UnityEngine.UI;
using System;



public class PushNotificationManager : MonoBehaviour
{
    readonly string CHANNEL_ID = "channelFcm";
    int apiLevel;
    public Text text;
    // Start is called before the first frame update
    void Start()
    {
#if UNITY_ANDROID
        // InitializeAndroidLocalPush();
        // InitializeAndroidFCM();
#endif
#if UNITY_IOS
		RequestAuthorization();
        InitializeIosFCM();       
#endif
    }

    //public void InitializeAndroidLocalPush()
    //{
    //    string androidInfo = SystemInfo.operatingSystem;
    //    Debug.Log("androidInfo: " + androidInfo);
    //    apiLevel = int.Parse(androidInfo.Substring(androidInfo.IndexOf("-") + 1, 2));
    //    Debug.Log("apiLevel: " + apiLevel);

    //    if (apiLevel >= 33 &&
    //        !Permission.HasUserAuthorizedPermission("android.permission.POST_NOTIFICATIONS"))
    //    {
    //        Permission.RequestUserPermission("android.permission.POST_NOTIFICATIONS");
    //    }

    //    if (apiLevel >= 26)
    //    {
    //        var channel = new AndroidNotificationChannel()
    //        {
    //            Id = CHANNEL_ID,
    //            Name = "test",
    //            Importance = Importance.High,
    //            Description = "for test",
    //        };
    //        AndroidNotificationCenter.RegisterNotificationChannel(channel);
    //    }
    //}

    //public void InitializeAndroidFCM()
    //{
    //    FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task =>
    //    {
    //        var dependencyStatus = task.Result;
    //        if (dependencyStatus == DependencyStatus.Available)
    //        {
    //            Debug.Log("Google Play version OK");

    //            FirebaseMessaging.TokenReceived += OnAndroidTokenReceived;
    //            FirebaseMessaging.MessageReceived += OnAndroidMessageReceived;
    //            FirebaseMessaging.RequestPermissionAsync().ContinueWithOnMainThread(task =>
    //            {
    //                Debug.Log("push permission: " + task.Status.ToString());
    //            });
    //            FirebaseMessaging.TokenRegistrationOnInitEnabled = true;
    //        }
    //        else
    //        {
    //            Debug.LogError(string.Format(
    //                "Could not resolve all Firebase dependencies: {0}",
    //                dependencyStatus
    //            ));
    //        }
    //    });
    //}

    //public void OnAndroidTokenReceived(object sender, TokenReceivedEventArgs token)
    //{
    //    Debug.Log("OnTokenReceived: " + token.Token);
    //}

    //public void OnAndroidMessageReceived(object sender, MessageReceivedEventArgs e)
    //{
    //    string type = "";
    //    string title = "";
    //    string body = "";

    //    // for notification message
    //    if (e.Message.Notification != null)
    //    {
    //        type = "notification";
    //        title = e.Message.Notification.Title;
    //        body = e.Message.Notification.Body;
    //    }
    //    // for data message
    //    else if (e.Message.Data.Count > 0)
    //    {
    //        type = "data";
    //        title = e.Message.Data["title"];
    //        body = e.Message.Data["body"];
    //    }
    //    Debug.Log("message type: " + type + ", title: " + title + ", body: " + body);

    //    var notification = new AndroidNotification();
    //    {
    //        notification.SmallIcon = "icon_0";
    //        notification.Title = title;
    //        notification.Text = body;
    //        notification.FireTime = DateTime.Now;
    //    }

    //    if (apiLevel >= 26)
    //    {
    //        AndroidNotificationCenter.SendNotification(notification, CHANNEL_ID);
    //        text.text = "title : " + notification.Title + "Text : " + notification.Text;
    //    }
    //    else
    //    {
    //        Debug.LogError("Android 8.0 이상의 디바이스에서만 푸시 알림이 정상적으로 표시됩니다.");
    //    }
    //}


    public IEnumerator<string> RequestAuthorization()
    {
        var req = new AuthorizationRequest(AuthorizationOption.Alert | AuthorizationOption.Badge, true);
        while (!req.IsFinished)
        {
            yield return null;
        }
    }

    public void InitializeIosFCM()
    {
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task =>
        {
            var dependencyStatus = task.Result;
            if (dependencyStatus == DependencyStatus.Available)
            {
                Debug.Log("Google Play version OK");

                FirebaseMessaging.TokenReceived += OnIosTokenReceived;
                FirebaseMessaging.MessageReceived += OnIosMessageReceived;
                FirebaseMessaging.RequestPermissionAsync().ContinueWithOnMainThread(task =>
                {
                    Debug.Log("push permission: " + task.Status.ToString());
                });
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

    public void OnIosTokenReceived(object sender, TokenReceivedEventArgs token)
    {
        Debug.Log("ontokenreceived: " + token.Token);
    }

    public void OnIosMessageReceived(object sender, MessageReceivedEventArgs e)
    {
        string type = "";
        string title = "";
        string body = "";
        int firetimeinseconds = 1;

        var timeTrigger = new iOSNotificationTimeIntervalTrigger()
        {
            TimeInterval = new TimeSpan(0, 0, firetimeinseconds),
            Repeats = false
        };

        // for notification message
        if (e.Message.Notification != null)
        {
            type = "notification";
            title = e.Message.Notification.Title;
            body = e.Message.Notification.Body;
        }
        // for data message
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
}
