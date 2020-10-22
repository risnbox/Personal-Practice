$(function () {
    $("#signin").on('click', GoogleLogin);
})

//Google登入 參考資料: https://dotblogs.com.tw/shadow/2019/01/31/113026 https://dotblogs.com.tw/shadow/2019/10/11/030306
function GoogleInit() {
    gapi.load('client:auth2', function () {
        gapi.client.init({
            clientId: Google_Client_id,
            scope: "https://www.googleapis.com/auth/user.phonenumbers.read https://www.googleapis.com/auth/user.gender.read",
            discoveryDocs: ["https://www.googleapis.com/discovery/v1/apis/people/v1/rest"]
        });
    })
}

function GoogleLogin() {
    var auth2 = gapi.auth2.getAuthInstance();
    auth2.signIn().then(function (GoogleUser) {
        let user_id = GoogleUser.getId();
        let AuthResponse = GoogleUser.getAuthResponse(true);
        let id_token = AuthResponse.id_token;
        gapi.client.people.people.get({
            'resourceName': 'people/me',
            'personFields': 'names,emailAddresses,genders,PhoneNumbers'
        }).then(function (res) {
            let str = JSON.stringify(res.result);
            var json = JSON.parse(str);
            var P = "";
            if (("phoneNumbers" in json)) { P = json.phoneNumbers[0].value }
            $('#alert').text('等待Google回應').show().delay(2500).fadeOut();
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