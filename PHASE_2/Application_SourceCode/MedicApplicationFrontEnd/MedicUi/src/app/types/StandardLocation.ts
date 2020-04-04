// Standard location object

class StandardLocation {
    country: string;
    location: string;
    geonamesId: number;
    googleId: string;
    coordinates; // object with latitude and longitude fields

    constructor(country: string,
                location: string,
                geonamesId: number = null,
                googleId: string = null,
                coordinates = null)
    {
        this.country = country;
        this.location = location;
        this.geonamesId = geonamesId;
        this.googleId = googleId;
        this.coordinates = coordinates;
    }
}

export default StandardLocation;
