# SENG3011_Medic

### Deliverable 1 (Tuesday 3rd March 2020)

1. Phase 1: Basic Structure of API Source Code in SENG3011_Medic/PHASE_1/API_SourceCode/MedicApi/MedicApi
2. Reports: 
     -  Management Information: Outlines Our team Responsibilities, Gannt Chart, Work Breakdown Schedule, Team Coordination   Tools,
     -  Design Details: States Our API Design Plan, Development Plan and Justification for Technologies Used

### Deliverable 2 (Tuesday 17th March 2020)

Part 1: Our API documentation can be accessed at https://seng3011medics.azurewebsites.net/swagger/index.html

### Final Github Breakdown
 1. SENG3011_Medic/PHASE_1
     - API_Documentation (Empty) documentation was handled in backend
     - API_SourceCode/MedicApi/MedicApi/Controllers
          * contains all of our endpoints
     - API_SourceCode/MedicApi/MedicApi/Models
          * contains all the data structures and DTOS used for backend
     - API_SourceCode/MedicApi/MedicApi/Resources
          * contains raw data for our location service
     - API_SourceCode/MedicApi/MedicApi/Services
          * contains our scrapers
          * contains all mappers used in the scrapers for searching locations/disease/symptoms/syndromes
          * contains API Logger service
          * contains Database storage service
     - API_SourceCode/MedicApi/MedicApi/Swashbuckle
           contains our templates and schemas for swagger documentation
     - API_SourceCode/MedicApi/MedicApi/TestData
          * contains our test inputs
     - API_SourceCode/MedicApi/MedicApi
          * root of project contains the startup for launch of api
     - TestScripts
          * contains the script to run our tests
     - TestScripts/Data
          * contains the input data for testing
     - TestScripts/Expected
          * contains the expected output for tests
     - TestScripts/Input
          * test cases using data from data folder
     - TestScripts/Output
          * all the output from running tests
2. SENG3011_Medic/PHASE_2
     - Application_Documentation (empty) documentation was handled inside the code
     - Application_SourceCode/MedicApplicationFrontEnd/MedicUi
          * root of our front end directory 
     - Application_SourceCode/MedicApplicationFrontEnd/MedicUi/src
          * contains testing files for front end
          * contains launch page of our application
     - Application_SourceCode/MedicApplicationFrontEnd/MedicUi/src/Services
          * contains location mapper service used for mapping locations
          * contains a service for formatting dates
          * contains a service for communicating between the map and the search component
     - Application_SourceCode/MedicApplicationFrontEnd/MedicUi/src/environments
          * contains global variables and "hard coded" values used across project
     - Application_SourceCode/MedicApplicationFrontEnd/MedicUi/src/app
          * contains our main application component, the root component on our application
     - Application_SourceCode/MedicApplicationFrontEnd/MedicUi/src/app/Components
          * contains map component
          * contains list view component
          * contains modal component
          * contains navigation bar component
          * contains search component
          * contains the marker view component
     - Application_SourceCode/MedicApplicationFrontEnd/MedicUi/src/app/Pages
          * contains home page which uses components from above
          * contains the statistics page 
          * contains the list view page using components from above
     - Application_SourceCode/MedicApplicationFrontEnd/MedicUi/src/app/apis/articles/base
          * contains base class for retrieving articles in a request
     - Application_SourceCode/MedicApplicationFrontEnd/MedicUi/src/app/apis/articles/concrete
          * contains all implementations for retrieving the articles using request above
     - Application_SourceCode/MedicApplicationFrontEnd/MedicUi/src/app/apis/articles/interfaces
          * contains the model of our standardised articles
     - Application_SourceCode/MedicApplicationFrontEnd/MedicUi/src/app/apis/articles/request
          * contains our standardised method of sending requests to multiple APIs
     - Application_SourceCode/MedicApplicationFrontEnd/MedicUi/src/app/apis/articles/test
          * contains testing for our API retrieval methods
     - Application_SourceCode/MedicApplicationFrontEnd/MedicUi/src/app/apis/articles/utils
          * helper methods for converting dates to standardised format for api retriever
     - Application_SourceCode/MedicApplicationFrontEnd/MedicUi/src/app/types
          * contains all our models used in the front end and components
3. Reports
     - Design Details.pdf
     - Final Report.pdf
     - Management Information.pdf
     - Testing Documentation.pdf
