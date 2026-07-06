# 🔔 PushNotification

> Unity Firebase 기반 Push Notification 구현 프로토타입

<br>

## 📖 프로젝트 개요

이 프로젝트는 Unity에서 Push Notification 기능을 구현하기 위해 제작한 프로토타입 프로젝트입니다.

<br>

## 🛠 기술 스택

| 분류 | 기술 |
|------|------|
| 엔진 | ![Unity](https://img.shields.io/badge/Unity_2022.3.29f1-000000?style=flat-square&logo=unity&logoColor=white) |
| 언어 | ![CSharp](https://img.shields.io/badge/C%23-239120?style=flat-square&logo=csharp&logoColor=white) |
| IDE | ![VisualStudio](https://img.shields.io/badge/Visual_Studio_2022-5C2D91?style=flat-square&logo=visualstudio&logoColor=white) |
| 패키지 | ![Firebase](https://img.shields.io/badge/Firebase_SDK_12.3.0-FFCA28?style=flat-square&logo=firebase&logoColor=black) ![MobileNotification](https://img.shields.io/badge/Mobile_Notification_2.3.2-000000?style=flat-square&logo=unity&logoColor=white) |
| 플랫폼 | ![Android](https://img.shields.io/badge/Android-3DDC84?style=flat-square&logo=android&logoColor=white) ![iOS](https://img.shields.io/badge/iOS-000000?style=flat-square&logo=apple&logoColor=white) |

<br>

## ⚙️ 초기 설정

- Unity Mobile Notification 2.3.2 → Package Manager를 통해 import
- Firebase Unity로 프로젝트 설정 및 패키지명 통일 (Unity app의 패키지명 - android/ios)
- `google-services.json` 및 `GoogleService-Info.plist` 다운로드 후 Assets 폴더에 저장
- Firebase Unity SDK 12.3.0 → Firebase Console에서 다운로드
  - Unity 프로젝트 상단에서 Assets > Import Package > Custom Package
  - `FirebaseAnalytics.unitypackage`, `FirebaseMessaging.unitypackage` import
- `MessagingUnityPlayerActivity`로 자동 변경
- `AndroidManifest.xml` 열기 후 아래 코드 추가

```xml
<!-- FirebaseAnalytics가 광고이기 때문에 구글 광고 관련 서비스 권한 제거 -->
<uses-permission android:name="com.google.android.gms.permission.AD_ID" tools:node="remove" />
<!-- 토큰 자동 초기화 방지 -->
<meta-data android:name="firebase_messaging_auto_init_enabled" android:value="false" />
<meta-data android:name="firebase_analytics_collection_enabled" android:value="false" />
```

<br>

## 🤖 Android 설정

1. Unity 프로젝트에서 keystore 설정 후 SHA-1 or SHA-256 등록
   - Player Settings > Publishing Settings > Keystore Manager > Create New > Anywhere

2. keystore가 설치된 경로에서 터미널 실행 후 SHA 확인:

```bash
keytool -list -v keystore 생성한_keyname.keystore
```

3. Firebase Console의 Android 앱 설정 > SHA 인증서 지문 등록
4. Unity Android 빌드 세팅 후 APK 설치
5. Firebase Console > Messaging 서비스에서 푸시 알림 테스트

<br>

## 🍎 iOS 설정

1. APN(Apple Push Notification) 설정
   - [Apple Developer](https://developer.apple.com) > Account > Certificates, Identifiers & Profiles > Keys > **+**
   - Apple Push Notifications service (APNs) 체크 > Register > Download (**다운로드는 1회만 가능**, 설치 경로 확인 필수)
   - Identifiers에서 앱 등록 > Services 중 Push Notification 체크

2. Firebase Console > iOS 앱 설정 > APN 인증 키 업로드 > `.p8` 업로드 및 Key ID, Team ID 입력

3. Unity iOS 빌드 후 Xcode에서 `.workspace` 파일 열기

4. General 탭에서 `UserNotifications.framework` 라이브러리 추가

5. Automatically manage signing 체크 (상황에 따라 미체크 가능)

6. Signing & Capabilities에서 **+** Capability > Push Notification 추가

7. Xcode에서 앱 빌드 후 실행

8. Firebase Console > Messaging 서비스에서 푸시 알림 테스트

<br>

## ⚠️ 보안 주의사항

아래 파일들은 **절대 GitHub에 올리면 안 돼요!**

| 파일 | 이유 |
|---|---|
| `google-services.json` | Firebase 프로젝트 키 포함 |
| `GoogleService-Info.plist` | Firebase iOS 키 포함 |
| `*.keystore` / `*.jks` | Android 서명 키 포함 |
| `*.p8` | Apple APNs 인증 키 포함 |
