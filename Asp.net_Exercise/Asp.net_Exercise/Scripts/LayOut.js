window.fbAsyncInit = function () {//FB初始化 登入及登出使用
    FB.init({
        appId: '920774955317229',
        cookie: true,
        status: true,
        xfbml: true,
        version: 'v12.0'
    });
};

$(() => {
    //Google Analytics
    window.dataLayer = window.dataLayer || [];
    function gtag() { dataLayer.push(arguments); }
    gtag('js', new Date());
    gtag('config', 'G-Y6MWMGK7MV');
    //---
    $('#logout').on('click', () => {
        $('#LayoutAlert').text('即將登出').show().delay(2000).fadeOut();
        FB.logout();
        setTimeout(() => {
            location.href = "/members/signout";
        }, 2000);
    })
})



