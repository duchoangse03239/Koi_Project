/* Custom Scripts */
$(document).ready(function () {
		
	// Start Header Animation
	$(window).scroll(function () {
	if ($(document).scrollTop() < 1) {
	$('header').removeClass('fix');
	} else {
	$('header').addClass('fix');
	}
	});

	// Start Parallax script
	$('#bannerArea').parallax("50%", 0.5);
	$('#peopleArea').parallax("50%", 0.5);
	$('#subscribeArea').parallax("50%", -0.5);
	
	// Start ToolTip
	$('[data-toggle=tooltip]').tooltip()
	
	// Start PoPover
	$('[data-toggle=popover]').popover()
	
	// Start Google Map
	$('#map_canvas').gmap({
		'center': new google.maps.LatLng(41.1329, -73.94434), // Change this to your desired latitude and longitude
		'zoom': 17,
		'mapTypeControl': false,
		'navigationControl': false,
		'streetViewControl': false,
		'styles': [{
			stylers: [{
				gamma: 0.60
			}, {
				hue: "#ffe168"
			}, {
				invert_lightness: false
			}, {
				lightness: 2
			}, {
				saturation: -20
			}, {
				visibility: "on"
			}]
		}]
	});
	var image = {
		url: 'images/marker.png', // Define the map marker file here
		// This marker is 51 pixels wide by 63 pixels tall.
		size: new google.maps.Size(51, 63),
		// The origin for this image is 0,0.
		origin: new google.maps.Point(0, 0),
		// The anchor for this image is the base of the flagpole at 26,63.
		anchor: new google.maps.Point(26, 63)
	};
	$('#map_canvas').gmap().bind('init', function () {
		$('#map_canvas').gmap('addMarker', {
			'id': 'marker-1',
			'position': '41.1329,-73.94434',
			'bounds': false,
			'icon': image
		}).click(function () {
			$('#map_canvas').gmap('openInfoWindow', {
				'content': '<p><strong>DreamTemplate</strong><br>8083 Middle Common<br>Hog Island, Utah</p>'
			}, this);
		});
	});  


});	