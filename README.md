# SENG3011_Medic

### Deliverable 1 (Tuesday 3rd March 2020)

1. Phase 1: Basic Structure of API Source Code in SENG3011_Medic/PHASE_1/API_SourceCode/MedicApi/MedicApi
2. Reports: 
     -  Management Information: Outlines Our team Responsibilities, Gannt Chart, Work Breakdown Schedule, Team Coordination   Tools,
     -  Design Details: States Our API Design Plan, Development Plan and Justification for Technologies Used

### Deliverable 2 (Tuesday 17th March 2020)

Part 1: Our API documentation can be accessed at https://seng3011medics.azurewebsites.net/swagger/index.html

### Final Github Breakdown
 Markup : 1. SENG3011_Medic/PHASE_1
     1. API_Documentation (Empty) documentation was handled in backend
     2. API_SourceCode/MedicApi/MedicApi/Controllers
          1. contains all of our endpoints
     3. API_SourceCode/MedicApi/MedicApi/Models
          1. contains all the data structures and DTOS used for backend
     4. API_SourceCode/MedicApi/MedicApi/Resources
          1. contains raw data for our location service
     5. API_SourceCode/MedicApi/MedicApi/Services
          1. contains our scrapers
          2. contains all mappers used in the scrapers for searching locations/disease/symptoms/syndromes
          3. contains API Logger service
          4. contains Database storage service
     6. API_SourceCode/MedicApi/MedicApi/Swashbuckle
          1. contains our templates and schemas for swagger documentation
     7. API_SourceCode/MedicApi/MedicApi/TestData
          1. contains our test inputs
     8. API_SourceCode/MedicApi/MedicApi
          1. root of project contains the startup for launch of api
     9. TestScripts
          1. contains the script to run our tests
     10. TestScripts/Data
          1. contains the input data for testing
     11. TestScripts/Expected
          1. contains the expected output for tests
     12. TestScripts/Input
          1. test cases using data from data folder
     13. TestScripts/Output
          1. all the output from running tests
2. SENG3011_Medic/PHASE_2
     1. Application_Documentation (empty) documentation was handled inside the code
     2. Application_SourceCode/MedicApplicationFrontEnd/MedicUi
          1. root of our front end directory 
     3. Application_SourceCode/MedicApplicationFrontEnd/MedicUi/src
          1. contains testing files for front end
          2. contains launch page of our application
     4. Application_SourceCode/MedicApplicationFrontEnd/MedicUi/src/Services
          1. contains location mapper service used for mapping locations
          2. contains a service for formatting dates
          3. contains a service for communicating between the map and the search component
     5. Application_SourceCode/MedicApplicationFrontEnd/MedicUi/src/environments
          1. contains global variables and "hard coded" values used across project
     6. Application_SourceCode/MedicApplicationFrontEnd/MedicUi/src/app
          1. contains our main application component, the root component on our application
     7. Application_SourceCode/MedicApplicationFrontEnd/MedicUi/src/app/Components
          1. contains map component
          2. contains list view component
          3. contains modal component
          4. contains navigation bar component
          5. contains search component
          6. contains the marker view component
     8. Application_SourceCode/MedicApplicationFrontEnd/MedicUi/src/app/Pages
          1. contains home page which uses components from above
          2. contains the statistics page 
          3. contains the list view page using components from above
     9. Application_SourceCode/MedicApplicationFrontEnd/MedicUi/src/app/apis/articles/base
          1. contains base class for retrieving articles in a request
     10. Application_SourceCode/MedicApplicationFrontEnd/MedicUi/src/app/apis/articles/concrete
          1. contains all implementations for retrieving the articles using request above
     11. Application_SourceCode/MedicApplicationFrontEnd/MedicUi/src/app/apis/articles/interfaces
          1. contains the model of our standardised articles
     12. Application_SourceCode/MedicApplicationFrontEnd/MedicUi/src/app/apis/articles/request
          1. contains our standardised method of sending requests to multiple APIs
     13. Application_SourceCode/MedicApplicationFrontEnd/MedicUi/src/app/apis/articles/test
          1. contains testing for our API retrieval methods
     14. Application_SourceCode/MedicApplicationFrontEnd/MedicUi/src/app/apis/articles/utils
          1. helper methods for converting dates to standardised format for api retriever
     15. Application_SourceCode/MedicApplicationFrontEnd/MedicUi/src/app/types
          1. contains all our models used in the front end and components
3. Reports
     1. Design Details.pdf
     2. Final Report.pdf
     3. Management Information.pdf
     4. Testing Documentation.pdf
          
