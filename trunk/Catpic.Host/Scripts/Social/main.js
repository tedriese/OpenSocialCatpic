(function(){
	$('.avatarbar').click(function(){
		$(this).addClass("avatarbar-expanded").attr("aria-selected", "true")
	});
	$('.avatarbar').mouseleave(function() {
	  $(this).removeClass("avatarbar-expanded").attr("aria-selected", "false")
	});
	$('.drawer-help').click(function(){
		
		$('.drawer').animate({height:'330px'}, 500);
		$('.drawer-container').css('height', '330px');;
		$('.drawer-slip').css('top', '-60px');
		
		$('.drawer-content').toggle();

	});
	
	
	$('.drawer').mouseleave(function() {
		//$('.drawer-help').toggle();
		//$('.drawer').animate({height:'60px'}, 500);
		
	});
	$('.drawer-close').click(function(){
		$('.drawer').animate({height:'60px'}, 500);
		$('.drawer-container').css('height', '60px');;
		$('.drawer-slip').css('top', '0px');
		$('.drawer-content').toggle();
	});
	
	$('.click').click(function(){
		//$('#nav1').animate({left:'-200px'}, 500);
		$('#nav2').animate({left:'60px'}, 500);

	});
	$('#nav2-back').click(function(){
		$('#nav1').animate({left:'0px'}, 500);
		$('#nav2').animate({left:'200px'}, 500);
	});
})();