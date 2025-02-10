//initialize the map
var map = L.map('map').setView([28.0781606, -80.6069429], 13);

var marker;

var streetMap = L.tileLayer('https://tile.openstreetmap.org/{z}/{x}/{y}.png', {
    attribution: 'Map data &copy; <a href="https://www.openstreetmap.org/copyright">OpenStreetMap</a> contributors, Imagery © <a href="https://www.mapbox.com/">Mapbox</a>',
    minZoom: 5,
    maxZoom: 18,
    id: 'mapbox/streets-v11',
    tileSize: 512,
    zoomOffset: -1,
    accessToken: 'pk.eyJ1IjoibWFwYm94IiwiYSI6ImNpejY4NXVycTA2emYycXBndHRqcmZ3N3gifQ.rJcFIG214AriISLbB6B5aw'
}).addTo(map);

//satelite view
var satMap = L.tileLayer('http://{s}.google.com/vt/lyrs=s&x={x}&y={y}&z={z}', {
    attribution: '&copy; <a href="https://www.openstreetmap.org/copyright">OpenStreetMap</a> contributors'
    , minZoom: 5
    , maxZoom: 18
    , tileSize: 512
    , zoomOffset: -1
    , subdomains: ['mt0', 'mt1', 'mt2', 'mt3']
})

//topographical view
var topMap = L.tileLayer(
    'https://basemap.nationalmap.gov/arcgis/rest/services/USGSTopo/MapServer/tile/{z}/{y}/{x}',
    {
        minZoom: 5,
        maxZoom: 16,
        attribution:
            'Tiles courtesy of the <a href="https://usgs.gov/">U.S. Geological Survey</a>',
    }
);

//hydrographical view
var hydroMap = L.tileLayer(
    'https://basemap.nationalmap.gov/arcgis/rest/services/USGSHydroCached/MapServer/tile/{z}/{y}/{x}',
    {
        minZoom: 5,
        maxZoom: 16,
        attribution:
            'Tiles courtesy of the <a href="https://usgs.gov/">U.S. Geological Survey</a>',
    }
);

//dark mode view
var darkLayer = L.tileLayer('https://{s}.basemaps.cartocdn.com/dark_all/{z}/{x}/{y}{r}.png', {
    attribution: '&copy; <a href="https://www.openstreetmap.org/copyright">OpenStreetMap</a> contributors &copy; <a href="https://carto.com/attributions">CARTO</a>',
    subdomains: 'abcd',
    minZoom: 5,
    maxZoom: 18
});

//pile together for control
var baseMaps = {
    "OpenStreetMap": streetMap,
    "Satellite": satMap,
    "Topographical": topMap,
    "HydroGraphical": hydroMap,
    "OSM Dark": darkLayer
};

var layerControl = L.control.layers(baseMaps).addTo(map);


//handles delete and its animation by assigning a class for css detection
export function deleteWithAnimation(taskID) {

    var rowElement = document.querySelector(`tr[data-id='${taskID}']`);

    rowElement.classList.add("fizzle");
    setTimeout(() => rowElement.remove(), 500);
}

//rebuilds and saves cooridnates of marker on movement
function markerDrag(e) {
    var result = marker.getLatLng();
    var content = marker.getPopup().getContent();
    var markerID = marker.options.customId
    console.log(result);
    map.removeLayer(marker)
    marker = new L.marker(result, { draggable: 'true', title: "Task Marker", zIndex: 950 }).addTo(map);
    marker.setZIndexOffset(950 - marker.y);
    marker.on('dragend', markerDrag);

    marker.bindPopup(content)
    marker.options.customId = markerID;

    marker.on('click', function () {
        marker.openPopup();
    });  

    //making a call to the api controller is one way to go around 
    //blazor js interop and something I am familiar with executing here
    fetch('/api/todolist/save-marker', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify({
            ItemID: marker.options.customId,
            Latitude: result.lat,
            Longitude: result.lng
        })
    })

    map.setView(marker.getLatLng(), 13);
};

//handles the loading of task markers
export function renderTaskMarker(task) {
    if (marker != null) {
        map.removeLayer(marker)
    }

    if (task.latitude || task.longitude != 0){
        marker = new L.marker([task.latitude, task.longitude], { draggable: 'true', title: "Task Marker", zIndex: 950 }).addTo(map);
        marker.on('dragend', markerDrag);
    }
    else {
        marker = new L.marker([28.0781606, -80.6069429], { draggable: 'true', title: "Task Marker", zIndex: 950 }).addTo(map);
        marker.on('dragend', markerDrag);
    }    

    map.setView(marker.getLatLng(), 13);

    marker.options.customId = task.itemID;

    marker.bindPopup(" ID: " + task.itemID + "<br> Task: " + task.name + "<br> Completed: " + task.completed)

    marker.on('click', function () {
        marker.openPopup();
    });  
}