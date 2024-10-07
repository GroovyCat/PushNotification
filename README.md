# PushNotification

## Development Content
+ 이 프로젝트는 유니티에서 Push Notification 기능을 구현하기 위해 제작한 프로토타입 프로젝트입니다.

## Configuration
+ Unity 2022.3.29f1
+ Mobile Notification 2.3.2 -> Package Manager를 통해 import
+ Firebase Unity로 프로젝트 설정 및 패키지명 통일(Unity app의 패키지명 - android/ios)
+ google-services.json 및 GoogleService-Info.plist 다운로드 후 Assets 폴더에 저장
+ Firebase Unity SDK 12.3.1 -> Firebase Console를 통해 다운로드
  + Unity 프로젝트 상단에 Assets > Import Package > Custom Package
  + FirebaseAnalytics.unitypackage, FirebaseMessaging.unitypackage import


### Android 설정
+ Unity 프로젝트에서 keystore 설정 후 SHA-1 or SHA-256 등록
  + Player Settings > publishing settings > keystore manager > Create New > Anywhere
  + 생성 후 keystore가 설치된 경로로 간 후 윈도우 기준 cmd 창 열기
```bash
keytool -list -v keystore 생성한 keyname.keystore
```

  + Firebase Console의 프로젝트의 안드로이드 앱 설정 > SHA 인증서 지문 설정
+ Firebase Console 프로젝트에서 Messaging 서비스를 통해 푸시 알림 테스트 진행


### iOS 설정
+ APN(Apple Push Notification) 설정 필요
  + 애플 개발자 홈페이지: https://developer.apple.com
  + Account > Certificates, Indentifiers & Profiles > Keys > **+** 클릭
  + Apple Push Notifications service (APNs) 체크 > Register > Download(다운로드는 한번만 가능) -> 설치 경로 꼭 확인
  + 설치 경로 .p8 파일이 있는 것을 확인하고 Identifiers에서 앱 등록 > service 중 Push Notification 체크
+ Firebase Console에서 iOS 앱 설정 > APN 인증 키 업로드 > .p8 업로드 및 Key ID, Team ID 입력
+ 유니티 iOS 필드 세팅 후 빌드 후 Xcode로 .workspace 파일 열기
+ General 탭에서 UserNotifications.framework 라이브러리 추가
+ Automatically manage signing 체크
+ Siging & Capabilities에서 **+** Capability 클릭 > Push Notification 추가
+ Xcode에서 앱 빌드 후 실행
