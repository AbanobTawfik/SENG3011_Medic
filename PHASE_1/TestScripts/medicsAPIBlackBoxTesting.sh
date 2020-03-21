echo SENG3011 Medics API Black Box Testing
echo Removing Previous Test Outputs ... ... ...
rm -r ActualOutputTest*

echo Test Case 1:
echo Start *REQ: 2015-01-01T00:00:00, End *Req: 2020-01-01T00:00:00
echo timezone: AEST, key_terms: anthrax,ebola,coronavirus
echo location: , max Default 25: , offset Default 0: 
echo -------------------------- Executing --------------------------------
curl -X GET "https://seng3011medics-staging.azurewebsites.net/api/Reports/GetArticles?start_date=2015-01-01T00%3A00%3A00&end_date=2020-01-01T00%3A00%3A00&timezone=AEST&key_terms=anthrax%2Cebola%2Ccoronavirus" -H "accept: */*" | python -m json.tool >> ActualOutputTest1.txt
echo -------------------------- Completed --------------------------------

echo Test Case 2:
echo Start *REQ: 2010-01-01T00:00:00, End *Req: 2020-01-01T00:00:00
echo timezone: AEST, key_terms: h1n1
echo location: , max Default 25: 10, offset Default 0:
echo -------------------------- Executing --------------------------------
curl -X GET "https://seng3011medics-staging.azurewebsites.net/api/Reports/GetArticles?start_date=2010-01-01T00%3A00%3A00&end_date=2020-01-01T00%3A00%3A00&timezone=AEST&key_terms=h1n1&max=10" -H "accept: */*" | python -m json.tool >> ActualOutputTest2.txt
echo -------------------------- Completed --------------------------------




