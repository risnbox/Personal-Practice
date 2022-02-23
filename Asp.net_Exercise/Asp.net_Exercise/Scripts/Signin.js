$(() => {
    $("#signin").on('click', GoogleLogin);
    $("#forget").on('click', function () {
        location.href = "/members/forgetpsw";
    })
    $("#signup").on('click', function () {
        location.href = "/members/signup";
    })
    let e = (msg) ? $("#alert").text(msg).show().delay(3000).fadeOut() : null;
})

function GoogleInit() {
    gapi.load('client:auth2', () => {
        gapi.client.init({
            clientId: Google_Client_id,//Signin.cshtml內將Web.config內的GoogleID透過Razor取出使用
            scope: "https://www.googleapis.com/auth/userinfo.profile",//想取用的參數內容
            discoveryDocs: ["https://www.googleapis.com/discovery/v1/apis/people/v1/rest"]//引用Google的js
        });
    })
}

function GoogleLogin() {
    let auth2 = gapi.auth2.getAuthInstance();//GoogleAuth物件
    auth2.signIn().then(() => {
        $('#alert').text('等待Google回應').show().delay(1000).fadeOut();
        //使用JS取得資料不需token
        //let AuthResponse = GoogleUser.getAuthResponse(true);//true則回傳token
        //let id_token = AuthResponse.id_token;//取得token
        gapi.client.people.people.get({//people.get方法參考：https://developers.google.com/people/api/rest/v1/people/get
            'resourceName': 'people/me',
            'personFields': 'names,emailAddresses,genders,PhoneNumbers'
        }).then(res => {
            let P = ("phoneNumbers" in res.result) ? res.result.phoneNumbers[0].value.replace(/ */g, "") : null;//搜尋是否包含手機號碼 若有則將值寫入變數
            let G = ("genders" in res.result) ? res.result.genders[0].value : null;
            $.ajax({
                url: "/api/membersapi/GooglesignUp",
                type: "post",
                data: {
                    gender: G,
                    email: res.result.emailAddresses[0].value,
                    name : res.result.names[0].displayName.replace(/ */,""),
                    phone: P,
                    Gid: res.result.resourceName.replace(/[people/]*/, "")
                    //id_token: id_token//要用後端存取才有需要
                },
                success: () => {
                    setTimeout(() => {
                        $('#alert').text('登入成功,即將轉至首頁').show().delay(1100).fadeOut();
                    }, 1100);
                    setTimeout(() => {
                        location.href = "/home/index";
                    }, 2000);
                },
                error: e => {
                    setTimeout(() => {
                    $("#alert").text(e.responseJSON.Message).show().delay(1100).fadeOut();
                    }, 1100);
                }
            });
        });
    });
}

//FB按鈕判定
function CheckFbConnected() {
    FB.getLoginStatus(function (response) {
        if (response.status == 'connected') {
            Fblogin();
        }
    });
}

function Fblogin() {
    FB.api('/me', { "fields": "id,name,email" }, function (response) {
        $('#alert').text("等待FB回應-Please wait for facebook to response. Don't click any button thank you.").show().delay(3000).fadeOut();
        $.ajax({
            url: "/api/membersapi/fblogin",
            type: "post",
            data: {
                fbid: response.id,
                name: response.name,
                email: response.email
            },
            success: () => {
                $('#alert').text('登入成功,即將轉至首頁').show().delay(2000).fadeOut();
                setTimeout(() => {
                    location.href = "/home/index";
                }, 2000);
            },
            error: e => {
                setTimeout(() => {
                    $("#alert").text(e.responseJSON.Message).show().delay(1100).fadeOut();
                }, 1100);
            }
        })
    });

}