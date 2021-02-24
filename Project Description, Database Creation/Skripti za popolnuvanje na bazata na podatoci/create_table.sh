#!/bin/python
import psycopg2
import sys

if len(sys.argv) < 2:
	print("You need to specify your PostgreSQL password!")
	print("EXAMPLE: ./create_table.sh 'password1'")
	sys.exit()

psw=sys.argv[1]

conn=psycopg2.connect(
	database="dians", user="postgres", password="psw", host='127.0.0.1',port='5432'
)

cursor=conn.cursor()

#droping Petrol_Station if exists
cursor.execute("DROP TABLE IF EXISTS Petrol_Stations")


#Creating desired table
sql='''CREATE TABLE Petrol_Stations (
	station_id SERIAL PRIMARY KEY,
	latitude DOUBLE PRECISION NOT NULL,
	longitude DOUBLE PRECISION NOT NULL,
	name VARCHAR(50) NOT NULL,
	name_en VARCHAR(50) ,
	opening_hours VARCHAR(50),
	fuel_diesel VARCHAR(50),
	fuel_biodiesel VARCHAR(50),
	fuel_octane95 VARCHAR(50),
	fuel_octane98 VARCHAR(50),
	fuel_octane100 VARCHAR(50),
	fuel_lpg VARCHAR(50)
)'''

#Execute the CREATE TABLE SQL command
cursor.execute(sql)
#push the changes on the server
conn.commit()

print("Successfully created table......")

#close the connection established with the server
conn.close()

