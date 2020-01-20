var mapKey = "AIzaSyDuYZTUdX5nRB8gfGw0x32iNo3yVm0O6qI";

$(document).ready(function () {
    // When the window has finished loading google map
    google.maps.event.addDomListener(window, "load", init);
});
function init(lat, lng) {
    var defLat = -0.2083997;
    var defLng = 51.5370912;
    if (currentCoords !== "") {
        defLat = currentCoords.split(",")[0];
        defLng = currentCoords.split(",")[1];
    }
    if (lat != null && lng != null) {
        defLat = lat;
        defLng = lng;
    }
    getFormattedAddress(defLat, defLng);
    var myLatlng = { lat: parseFloat(defLat), lng: parseFloat(defLng) };
    // MAP OPTIONS
    var mapOptions = {
        zoom: 14,
        center: myLatlng,
        // Style for Google Maps
        styles: [{ "featureType": "landscape", "stylers": [{ "saturation": -100 }, { "lightness": 65 }, { "visibility": "on" }] }, { "featureType": "poi", "stylers": [{ "saturation": -100 }, { "lightness": 51 }, { "visibility": "simplified" }] }, { "featureType": "road.highway", "stylers": [{ "saturation": -100 }, { "visibility": "simplified" }] }, { "featureType": "road.arterial", "stylers": [{ "saturation": -100 }, { "lightness": 30 }, { "visibility": "on" }] }, { "featureType": "road.local", "stylers": [{ "saturation": -100 }, { "lightness": 40 }, { "visibility": "on" }] }, { "featureType": "transit", "stylers": [{ "saturation": -100 }, { "visibility": "simplified" }] }, { "featureType": "administrative.province", "stylers": [{ "visibility": "off" }] }, { "featureType": "water", "elementType": "labels", "stylers": [{ "visibility": "on" }, { "lightness": -25 }, { "saturation": -100 }] }, { "featureType": "water", "elementType": "geometry", "stylers": [{ "hue": "#ffff00" }, { "lightness": -25 }, { "saturation": -97 }] }]
    };
    // Get all html elements for map
    var mapElement = document.getElementById("dvMap");
    // MAP
    // Create the Google Map using elements
    var map = new google.maps.Map(mapElement, mapOptions);
    // MARKER
    var marker = new google.maps.Marker({
        map: map,
        draggable: true,
        position: myLatlng,
        title: "Click to Zoom"
    });
    // MAP CLICK
    google.maps.event.addListener(map, "click", function (e) {
        var latMap = e.latLng.lat();
        var lngMap = e.latLng.lng();
        setCoordinates(latMap, lngMap);
        getFormattedAddress(latMap, lngMap);
    });
    // MARKER DRAGEND
    google.maps.event.addListener(marker, "dragend", function () {
        var latMarker = marker.getPosition().lat();
        var lngMarker = marker.getPosition().lng();
        setCoordinates(latMarker, lngMarker);
        getFormattedAddress(latMarker, lngMarker);
    });
}
$(document).ready(function () {
    $("#btnGetByAddress").click(
        function getCoordinates() {
            var address = getAddress();
            if (address === null || address === "") {
                swal({
                    title: "Error!",
                    text: "Full Address field cannot be empty!",
                    type: "error"
                });
                return;
            }
            $.ajax({
                url: "https://maps.googleapis.com/maps/api/geocode/json?key=" + mapKey + "&address=" + address
            }).done(function (data) {
                if (data.results.length > 0) {
                    var lat = data.results[0].geometry.location.lat;
                    var lng = data.results[0].geometry.location.lng;
                    setCoordinates(lat, lng);
                    getFormattedAddress(lat, lng);
                    init(lat, lng);
                } else {
                    swal({
                        title: "Not found!",
                        text: "No location found",
                        type: "error"
                    });
                }
            });
        });
});
function setCoordinates(lat, lng) {
    $("#VenueCoordinates").val(lat + ", " + lng);
}
function getAddress() {
    return $("#VenueAddress").val();
}
function setAddress(address) {
    $("#VenueAddress").val(address);
}
function getFormattedAddress(lat, lng) {
    $.ajax({
        url: "https://maps.googleapis.com/maps/api/geocode/json?key=" + mapKey + "&latlng=" + lat + "," + lng
    }).done(function (data) {
        if (data.results !== undefined && data.results.length > 0) {
            setAddress(data.results[0].formatted_address);
        }
    });
}
$(function () {
    $("#VenueAddress").keydown(function (e) {
        if (e.keyCode === 13) {
            $("#btnGetByAddress").focus().click();
            return false;
        }
    });
});
$(document).ready(function () {
    if (currentCoords === "") {
        $("#VenueAddress").val(eventCity);
        $("#btnGetByAddress").click();
    }
});