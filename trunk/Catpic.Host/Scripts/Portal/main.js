(function(){
	$('.current-locale').click(function(){
		var panel = $('.locale-selection-panel');
		panel.show();
		panel.css('top','-110px');
	});
	$('.locale-selection-panel').mouseleave(function(){
		$(this).hide();
	});
})();