using Firebase.Messaging;
using Firebase;
using System.Collections.Generic;
using UnityEngine;
using Firebase.Extensions;
// using UnityEngine.Android;
using Unity.Notifications.iOS;
// using Unity.Notifications.Android;
using UnityEngine.UI;



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
    //        notification.FireTime = System.DateTime.Now;
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


    public ienumerator<string> requestauthorization()
    {
        var req = new authorizationrequest(authorizationoption.alert | authorizationoption.badge, true);
        while (!req.isfinished)
        {
            yield return null;
        }
    }

    public void initializeiosfcm()
    {
        firebaseapp.checkandfixdependenciesasync().continuewith(task =>
        {
            var dependencystatus = task.result;
            if (dependencystatus == dependencystatus.available)
            {
                firebasemessaging.tokenreceived += oniostokenreceived;
                firebasemessaging.messagereceived += oniosmessagereceived;
                firebasemessaging.requestpermissionasync().continuewithonmainthread(task =>
                {
                    debug.log("push permission: " + task.status.tostring());
                });
                firebasemessaging.tokenregistrationoninitenabled = true;
            }
            else
            {
                debug.logerror(string.format(
                    "could not resolve all firebase dependencies: {0}",
                    dependencystatus
                ));
            }
        });
    }

    public void oniostokenreceived(object sender, tokenreceivedeventargs token)
    {
        debug.log("ontokenreceived: " + token.token);
    }

    public void oniosmessagereceived(object sender, messagereceivedeventargs e)
    {
        string type = "";
        string title = "";
        string body = "";
        int firetimeinseconds = 1;

        var timetrigger = new iosnotificationtimeintervaltrigger()
        {
            timeinterval = new system.timespan(0, 0, firetimeinseconds),
            repeats = false
        };

        // for notification message
        if (e.message.notification != null)
        {
            type = "notification";
            title = e.message.notification.title;
            body = e.message.notification.body;
        }
        // for data message
        else if (e.message.data.count > 0)
        {
            type = "data";
            title = e.message.data["title"];
            body = e.message.data["body"];
        }
        debug.log("message type: " + type + ", title: " + title + ", body: " + body);

        var notification = new iosnotification()
        {
            identifier = "hello_world_notification",
            title = title,
            body = body,
            subtitle = type,
            showinforeground = true,
            foregroundpresentationoption = (presentationoption.alert | presentationoption.sound),
            categoryidentifier = "default_category",
            threadidentifier = "thread1",
            trigger = timetrigger

        };

        iosnotificationcenter.schedulenotification(notification);
    }
}
