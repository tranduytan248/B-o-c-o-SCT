

// Your web app's Firebase configuration
var firebaseConfig = {
    apiKey: "AIzaSyDKYvNJ5ofS4M7DhrepslPWgjBKi2U_mH8",
    authDomain: "thuphimoitruong-11095.firebaseapp.com",
    databaseURL: "https://thuphimoitruong-11095.firebaseio.com",
    projectId: "thuphimoitruong-11095",
    storageBucket: "thuphimoitruong-11095.appspot.com",
    messagingSenderId: "809413040372",
    appId: "1:809413040372:web:536371834b75bebc"
};
// Initialize Firebase
var FB = firebase.initializeApp(firebaseConfig);

// Retrieve Firebase Messaging object.
var messaging = FB.messaging();
messaging.usePublicVapidKey("BNTLygBMyIthUpR_dIEXl1hdtXp6583fpJc2jC9IIbcStalIxcT-zUvjYGEGHOmo8q02VA1qqORxpIWOghzil4s");

if ("serviceWorker" in navigator) {
    navigator.serviceWorker.register("../firebase-messaging-sw.js")
        .then(function(registration) {
            console.log("Registration successful, scope is:", registration.scope);
        }).catch(function(err) {
            console.log("Service worker registration failed, error:", err);
        });
}

messaging.requestPermission()
    .then(function() {
        console.log("Have Permission.");
        return messaging.getToken();
    })
    .then(function(token) {
        console.log(token);
    })
    .catch(function(error) {
        console.log(error);
    });

//messaging.onMessage(function (payload) {
//    console.log('[firebase-messaging-sw.js] Received background message ', payload);
//    // ...
//});

//messaging.setBackgroundMessageHandler(function (payload) {
//    console.log('[firebase-messaging-sw.js] Received background message ', payload);
//    // Customize notification here
//    var notificationTitle = 'Background Message Title';
//    var notificationOptions = {
//        body: 'Background Message body.',
//        icon: '/firebase-logo.png'
//    };

//    return self.registration.showNotification(notificationTitle,
//      notificationOptions);
//});

//Notification.requestPermission().then(function (permission) {
//    if (permission === 'granted') {
//        console.log('Notification permission granted.');
//        // TODO(developer): Retrieve an Instance ID token for use with FCM.
//        // ...
//        // Get Instance ID token. Initially this makes a network call, once retrieved
//        // subsequent calls to getToken will return from cache.
//        messaging.getToken().then(function (currentToken) {
//            if (currentToken) {
//                sendTokenToServer(currentToken);
//                updateUIForPushEnabled(currentToken);
//            } else {
//                // Show permission request.
//                console.log('No Instance ID token available. Request permission to generate one.');
//                // Show permission UI.
//                updateUIForPushPermissionRequired();
//                setTokenSentToServer(false);
//            }
//        }).catch(function (err) {
//            console.log('An error occurred while retrieving token. ', err);
//            showToken('Error retrieving Instance ID token. ', err);
//            setTokenSentToServer(false);
//        });
//    } else {
//        console.log('Unable to get permission to notify.');
//    }
//});    