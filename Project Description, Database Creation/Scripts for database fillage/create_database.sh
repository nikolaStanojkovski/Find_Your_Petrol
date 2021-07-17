#!/bin/python
import psycopg2
import sys

if len(sys.argv) < 2:
	print("You must provide password as an argument!")
	print("EXAMPLE: ./create_database.sh 'password1'")
	sys.exit()

psw=sys.argv[1]

#establish connection
conn=psycopg2.connect(
	database="postgres", user='postgres', password="psw", host='127.0.0.1', port='5432'
)
conn.autocommit=True

cursor=conn.cursor()

#Checking if database exists
cursor.execute('DROP DATABASE IF EXISTS DIANS')
#Preparing query to create database
sql='''CREATE DATABASE DIANS'''
#Creating a database
cursor.execute(sql)

print("Database created successfully.....")

conn.close()

