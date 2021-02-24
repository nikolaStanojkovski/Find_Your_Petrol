#!/bin/python
import psycopg2
import xml.etree.cElementTree as ET
import sys

if len(sys.argv) < 2:
	print("You need to specify your password to the PostgreSQL server!")
	print("EXAMPLE: ./xml_extract_data_python.sh 'password123'")
	sys.exit()

psw=sys.argv[1]

#construct tree using the xml file
tree=ET.ElementTree(file='map.xml')
root=tree.getroot()

#Setting connection with database
conn=psycopg2.connect(
	database="dians", user='postgres', password="psw", host='127.0.0.1', port='5432'
)

conn.autocommit=True

cursor=conn.cursor()

for e in root:
	if e.tag != 'node':
		continue
	lat=e.get('lat')
	lon=e.get('lon')
	name="Petrol Station"
	name_en="Unknown"
	opening_hours="Unknown"
	diesel="Unknown"
	biodiesel="Unknown"
	octane95="Unknown"
	octane98="Unknown"
	octane100="Unknown"
	lpg="Unknown"
	for tag in e:
		if tag.get('k')=='name' or tag.get('k')=='brand':
			name=tag.get('v')
		elif tag.get('k')=='name:en':
			name_en=tag.get('v')
		elif tag.get('k')=='opening_hours':
			opening_hours=str(tag.get('v'))
		elif tag.get('k')=='fuel:diesel':
			diesel=tag.get('v')
		elif tag.get('k')=='fuel:biodiesel':
			biodiesel=tag.get('v')
		elif tag.get('k')=='fuel:octane_95':
			octane95=tag.get('v')
		elif tag.get('k')=='fuel:octane_98':
			octane98=tag.get('v')
		elif tag.get('k')=='fuel:octane_100':
			octane100=tag.get('v')
		elif tag.get('k')=='fuel:lpg':
			lpg=tag.get('v')

	cursor.execute("INSERT INTO Petrol_Stations(latitude, longitude, name, name_en, opening_hours, fuel_diesel, fuel_biodiesel, fuel_octane95, fuel_octane98, fuel_octane100, fuel_lpg) VALUES (%s, %s, %s, %s, %s, %s, %s, %s, %s, %s, %s)", (lat,lon,name,name_en,opening_hours,diesel,biodiesel,octane95,octane98,octane100,lpg))

conn.commit()

print("Successfully filled table.....")

conn.close()
