
$('.scroll').click(function(event) {
    $('html, body').animate({
        scrollTop: $($.attr(this, 'href')).offset().top - 50
    }, 750);
    event.preventDefault();
});
$('.goTop').click(function(event) {
    $('body, html').stop(true).animate({scrollTop:0},750);
    event.preventDefault();
});


// 縮放特效
// var $collapse = $("[data-toggle='collapse']");
// var fall = '.collapse-content';

// $collapse.click(function () {
//     $(this).next(fall).slideToggle();
//     $(this).parent().siblings().children().next().slideUp(100);
//     // $(this).siblings().next(fall).slideUp();
//     $(this).toggleClass("current"),
//             $collapse.not(this).removeClass("current");
//     return false;
// });

// 行動裝置的主選單
$trigger = $('.mobile-trigger');
$menuClose = $('.mobile-close');

$trigger.click(function() {
    $(this).toggleClass('active');
    $('body').toggleClass('overlay');
});
$menuClose.click(function() {
    $('body').removeClass('overlay');
});

// 行動裝置的產品分類選單
// $(".pro-menu").click(function() {
//     $(this).toggleClass("active");
//     // $('aside nav').slideToggle(750);
//     $('.sidebar').slideToggle(170);
//     $(fall).slideUp();
//     $collapse.removeClass("current");
// });

// 搜尋框縮放
// var submitIcon = $("[data-expand='icon']");
// var inputBox = $("[data-expand='input']");
// var searchBox = $("[data-expand='box']");
// var isOpen = false;
// submitIcon.click(function(){
//     if(isOpen == false){
//         searchBox.addClass('search-open');
//         inputBox.focus();
//         isOpen = true;
//     } else {
//         searchBox.removeClass('search-open');
//         inputBox.focusout().val('');
//         $("[data-expand='btn']").removeClass('enter');
//         isOpen = false;
//     }
// });
// submitIcon.mouseup(function(){
//     return false;
// });
// searchBox.mouseup(function(){
//     return false;
// });
// $(document).mouseup(function(){
//     if(isOpen == true){
//         $("[data-expand='icon']").css('display','block');
//         submitIcon.click();
//     }
// });

// function buttonUp(){
//     var inputVal = $("[data-expand='input']").val();
//     inputVal = $.trim(inputVal).length;
//     if( inputVal !== 0){
//         $("[data-expand='icon']").css('display','none');
//         $("[data-expand='btn']").addClass('enter');
//     } else {
//         $("[data-expand='input']").val('');
//         $("[data-expand='icon']").css('display','block');
//         $("[data-expand='btn']").removeClass('enter');
//     }
// }
