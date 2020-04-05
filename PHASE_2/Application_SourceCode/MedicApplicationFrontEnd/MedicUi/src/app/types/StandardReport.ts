// Standard report object

import StandardLocation from './StandardLocation';

class StandardReport {
    diseases: string[];
    syndromes: string[];
    eventDate: string;
    locations: StandardLocation[];

    constructor(diseases: string[],
                syndromes: string[],
                eventDate: string, 
                locations: StandardLocation[])
    {
        this.diseases = diseases;
        this.syndromes = syndromes;
        this.eventDate = eventDate;
        this.locations = locations;
    }
}

export default StandardReport;
