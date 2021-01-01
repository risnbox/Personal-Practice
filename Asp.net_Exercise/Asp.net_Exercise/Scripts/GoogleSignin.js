$(function () {
    $("#signin").on('click', GoogleLogin);
    $("#forget").on('click', function () {
        location.href = "/members/forgetpsw";
    })
})

//Google登入 參考資料: https://dotblogs.com.tw/shadow/2019/01/31/113026 https://dotblogs.com.tw/shadow/2019/10/11/030306
function GoogleInit() {
    gapi.load('client:auth2', function () {
        gapi.client.init({
            clientId: Google_Client_id,//Signin.cshtml內將Web.config內的GoogleID透過Razor取出使用
            //想取用的參數內容 下列為手機號碼及性別(使用者公開且有填值我才抓的到)
            scope: "https://www.googleapis.com/auth/user.phonenumbers.read https://www.googleapis.com/auth/user.gender.read",
            discoveryDocs: ["https://www.googleapis.com/discovery/v1/apis/people/v1/rest"]//引用Google的js
        });
    })
}

function GoogleLogin() {
    var auth2 = gapi.auth2.getAuthInstance();//取得GoogleAuth物件
    auth2.signIn().then(function (GoogleUser) {
        let user_id = GoogleUser.getId();//得到使用者ID
        let AuthResponse = GoogleUser.getAuthResponse(true);//true則回傳token
        let id_token = AuthResponse.id_token;//取得token
        gapi.client.people.people.get({//people.get方法參考：https://developers.google.com/people/api/rest/v1/people/get
            'resourceName': 'people/me',
            'personFields': 'names,emailAddresses,genders,PhoneNumbers'
        }).then(function (res) {
            let str = JSON.stringify(res.result);//用於從一個對象解析出字符串
            var json = JSON.parse(str);//用於從一個字符串中解析出json對象
            var P = "";
            if (("phoneNumbers" in json)) { P = json.phoneNumbers[0].value }//搜尋是否包含手機號碼 若有則將值寫入變數
            $('#alert').text('等待Google回應').show().delay(2500).fadeOut();//浮動視窗
            $.ajax({
                url: "/api/membersapi/GooglesignUp",
                type: "post",
                data: {
                    gender: json.genders[0].value,
                    phone: P,
                    id_token: id_token
                },
                success: e => {
                    alert("登入成功,即將轉至首頁");
                    location.href = "/home/index";
                },
                error: e => {
                    console.log(e);
                    alert(e.responseJSON.Message);
                    if (e.responseJSON.Message == "Google帳號未含手機號碼,請至會員中心填寫") {
                        location.href = "/members/membercenter";
                    }
                }
            });
        });
    })
}