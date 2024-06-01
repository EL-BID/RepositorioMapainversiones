// Desplegar Geojson en el mapa 
function presentMap(featureCollectionString, defaultLatitude, defaultLongitude) {
    if (featureCollectionString?.length) {
        let featureCollection = JSON.parse(featureCollectionString);

        // Clear previous layers
        geometryLayerGroup.clearLayers();

        // Add new geometries to the LayerGroup
        featureCollection.features.forEach(function (feature) {
            let layer = L.geoJSON(feature).addTo(map);
            geometryLayerGroup.addLayer(layer);
        });


        // Fly to the bounds of the new data
        let bounds = L.markers.getBounds();
        console.log('No llega aqui, saca error en getBounds');
        if (bounds.isValid()) {
            map.flyToBounds(bounds);
        }
    } else {
        // No data, use default coordinates
        map.setView([defaultLatitude, defaultLongitude], 13);
    }
}
