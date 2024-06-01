


// vars
let mapTextContainer = null;
let map = null;

// ==================================================
//
//   Capture/Display Map
//   function to be invoked from form.
//
function captureMap(imageContainer, textContainer, isEditable, defaultLatitude, defaultLongitude) {
    // prepare map environment
    if (imageContainer?.length > 0) {
        mapTextContainer = `#${textContainer}`;
        displayMap(imageContainer, $(mapTextContainer).text(), isEditable, defaultLatitude, defaultLongitude);
    }
}

//   Display Map
function displayMap(imageContainer, featureCollection, isEditable, defaultLatitude, defaultLongitude) {
    // prepare map environment
    if (imageContainer?.length > 0) {
        let centro = [defaultLatitude, defaultLongitude];
        let zoom = 13;
        // mapa
        map = L.map(imageContainer).setView(centro, zoom);
        map.pm.setLang("es");

        // Set Basemap:
        L.tileLayer('http://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png', {
            attribution: '&copy; <a href="https://www.openstreetmap.org/copyright">OpenStreetMap</a> contributors'
        }).addTo(map);

        if (isEditable) {

            // geocoder:
            L.Control.geocoder({
                defaultMarkGeocode: false,
                collapsed: true, // para que aparezca abierto o cerrado
                placeholder: 'Buscar..'
            })
                .on('markgeocode', (e) => {
                    let bbox = e.geocode.bbox;
                    let poly = L.polygon([
                        bbox.getSouthEast(),
                        bbox.getNorthEast(),
                        bbox.getNorthWest(),
                        bbox.getSouthWest()
                    ]);
                    map.fitBounds(poly.getBounds());
                })
                .addTo(map);
        }

        setControls(isEditable);

        presentMap(featureCollection);
    }
}

// Desplegar Geojson en el mapa 
function presentMap(featureCollectionString) {
    // load
    // <-- convert geojson from db, and display on map
    if (featureCollectionString?.length && featureCollectionString != '{"type":"FeatureCollection","features":[]}') {
        //'{"type":"FeatureCollection","features":[{"type":"Feature","properties":{ },"geometry":{"type":"Point","coordinates":[-58.381691,-34.550478]}},{"type":"Feature","properties":{ },"geometry":{"type":"Point","coordinates":[-58.326588,-34.545388]}},{"type":"Feature","properties":{ },"geometry":{"type":"Point","coordinates":[-58.328304,-34.567866]}}]}';
        let featureCollection = JSON.parse(featureCollectionString);
        let geometryLayerGroup = L.layerGroup();  // LayerGroup para almacenar las geometrías

        // Agregar geometrías iniciales al LayerGroup
        featureCollection.features.forEach(function (feature) {
            // itera por cada feature del feature collection,
            // crea una capa tipo geojson, y la agrega al mapa
            let layer = L.geoJSON(feature).addTo(map);
            geometryLayerGroup.addLayer(layer);
        });

        // Calculate bounds of all features in the LayerGroup
        let bounds = getLayerGroupBounds(geometryLayerGroup);
        if (bounds.isValid()) {
            map.flyToBounds(bounds);
        }

    }
}


// obtener el FeatureCollection
function getMapInfo() {
    if (mapTextContainer != null) {
        let featureCollection = map.pm.getGeomanLayers(true).toGeoJSON();
        let featureCollectionStringify = JSON.stringify(featureCollection);
        //actualiza en el input el resultado
        $(mapTextContainer).text(featureCollectionStringify);
    }
}




// Helper function to get bounds of a LayerGroup
function getLayerGroupBounds(layerGroup) {
    let groupBounds = null;

    layerGroup.eachLayer(function (layer) {
        if (groupBounds === null) {
            groupBounds = layer.getBounds();
        } else {
            groupBounds.extend(layer.getBounds());
        }
    });

    return groupBounds;
}


// sets available controls depending on mode (isEditable: true=edit false=display)
function setControls(isEditable) {

    // Agregar el plugin Leaflet-Geoman para edición
    if (isEditable) {
        // set parameters for edit
        map.pm.addControls({
            drawText: false,
            drawCircleMarker: false,
            drawCircle: false,
            drawRectangle: false,
            drawPolyline: true,
            drawMarker: true,
            drawPolygon: true,
            editMode: true,
            dragMode: false,
            scrollmode: false,
            cutPolygon: false,
            removalMode: true,
            rotateMode: false
        });
    }
    else {
        // set parameters for display
        map.pm.addControls({
            drawText: false,
            drawCircleMarker: false,
            drawCircle: false,
            drawRectangle: false,
            drawPolyline: false,
            drawMarker: false,
            drawPolygon: false,
            editMode: false,
            dragMode: false,
            cutPolygon: false,
            removalMode: false,
            rotateMode: false
        });

    }
}

// Switch Base Map
function switchBasemap() {

    // new basemap switch:
    new L.basemapsSwitcher([
        {
            layer: L.tileLayer('http://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png', {
                attribution: '&copy; <a href="https://www.openstreetmap.org/copyright">OpenStreetMap</a> contributors'
            }).addTo(map), //DEFAULT MAP
            icon: '/assets/img/map/osm_logo.png',
            name: 'OSM'
        },
    ], { position: 'topright' }).addTo(map);

}


