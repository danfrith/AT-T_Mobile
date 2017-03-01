using UnityEngine;
using System.Collections;
using System.Collections.Generic;


using Amazon;
using Amazon.CognitoSync;
using Amazon.Runtime;
using Amazon.CognitoIdentity;
using Amazon.CognitoIdentity.Model;
using Amazon.CognitoSync.SyncManager;

public class SocialMediaManager : MonoBehaviour {

	public string ShareIntentTitle = "Share the game";
	public string TwitterMessage = "Some text goes here for twitter";
	public string FacebookMessage = "Some text goes here for facebook";
	public string GooglePlusMessage = "Some text goes here google plus";

    public Texture2D ShareImage;

	public void ButtonFacebookPressed()
	{
		Debug.Log ("ButtonFacebookPressed");

		#if UNITY_ANDROID 
			Debug.Log ("AndroidFacebook");	// Test this on a mobile with FB installed.
			AndroidSocialGate.StartShareIntent(ShareIntentTitle, FacebookMessage, ShareImage, "facebook.katana");
#endif

#if UNITY_IPHONE
			Debug.Log ("IOSFacebook");	
			IOSSocialManager.Instance.FacebookPost(FacebookMessage);
#endif

        //		SPFacebook.Instance.FeedShare (
        //			link: "https://example.com/myapp/?storyID=thelarch",
        //			linkName: "The Larch",
        //			linkCaption: "I thought up a witty tagline about larches",
        //			linkDescription: "There are a lot of larch trees around here, aren't there?",
        //			picture: "https://example.com/myapp/assets/1/larch.jpg"
        //		);
        //
        //		SPFacebook.Instance.AppRequest("Come play this great game!");
    }

    public void ButtonTwitterPressed()
	{
		Debug.Log ("ButtonTwitterPressed");	
		#if UNITY_ANDROID 
			Debug.Log ("AndroidTwitter");
			AndroidSocialGate.StartShareIntent(ShareIntentTitle, TwitterMessage, "twi");
#endif

#if UNITY_IPHONE
			Debug.Log ("IOSTwitter");	
		    IOSSocialManager.Instance.TwitterPost(TwitterMessage);
#endif

        //		if (AndroidTwitterManager.Instance.IsAuthed)
        //			AndroidTwitterManager.Instance.PostWithAuthCheck("Hello, I'am posting this from my app");
        //		else
        //			AndroidSocialGate.StartShareIntent("Hello Share Intent", "This is my text to share",  "twi");
        //
    }

    public void ButtonGooglePlusPressed()
	{
		Debug.Log ("ButtonGooglePlusPressed");	
		#if UNITY_ANDROID 
			Debug.Log ("AndroidTwitter");	
			//AndroidSocialGate.StartShareIntent(ShareIntentTitle, TwitterMessage, "twi");
			AndroidSocialGate.StartGooglePlusShare(GooglePlusMessage, ShareImage);
		#endif

		#if UNITY_IPHONE
			Debug.Log ("IOSTwitter");	
			// ios native call
		#endif
	}


}
