// 頁籤切換
var tab = $('.js-tab');
var tabContent = '.tab-content';
$('.tab-content:gt(0)').hide();
// $(tab.eq(0).addClass('active').attr('href')).siblings(tabContent).hide();
tab.click(function () {
    event.preventDefault();
    $($(this).attr('href')).fadeIn().siblings(tabContent).hide();
    $(this).toggleClass('active');
    tab.not(this).removeClass('active');
});