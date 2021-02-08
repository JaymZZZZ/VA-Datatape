Imports Microsoft.VisualBasic
Imports System
Imports System.Collections
Imports System.Collections.Generic
Imports System.Data
Imports System.Drawing
Imports System.Diagnostics
Imports System.Windows.Forms
Imports System.Linq
Imports System.Xml.Linq
Imports System.Threading.Tasks
Imports System.IO
Imports System.Text.RegularExpressions


Public Class VAInline


	Public Sub Main()
	
	' Set the directory locations that we want to search in
	Dim dir_locations = {
	Environ$("USERPROFILE")+"\Saved Games\DCS.openbeta\Datatape", 
	Environ$("USERPROFILE")+"\Saved Games\DCS\Datatape", 
	"C:\Program Files (x86)\VoiceAttack\Datatape"
	}
	
	' directory and file related variables
	Dim dir_found as Boolean = false
	Dim datatape_dir as String	
	Dim dir_loc as integer = 0
	
	Dim file_found as Boolean = false
	Dim datatape_file as string
	
	Dim file_code as string
	
	Dim pos as integer = 2
	Dim current_pos as string
	
	
	' Look for the available directories and pick the one that works the best based on the order within the list
	while dir_found = false and dir_loc < dir_locations.length
		
		If Directory.Exists(dir_locations(dir_loc)) Then
			dir_found = true
			datatape_dir = dir_locations(dir_loc)
			VA.WriteToLog("Reading data from directory: "+datatape_dir, "blue")
		end if
		dir_loc = dir_loc + 1
		
	end while
	
	' If no directory was found, then it needs to be created/fixed. Output some insturctions on how to do so.
	if dir_found = false then
		VA.WriteToLog("Advanced File Format: datatape_alpha_<ANYTHING_YOU_WANT_HERE>.txt", "red")
		VA.WriteToLog("Basic File Format: datatape.txt", "red")
		VA.WriteToLog("File name must match one of the following formats:", "red")
		VA.WriteToLog(dir_locations(2), "red")
		VA.WriteToLog(dir_locations(1), "red")
		VA.WriteToLog(dir_locations(0), "red")	
		VA.WriteToLog("In order for datatape to run, you need to create a datatape file in one of the following locations:", "red")	
		VA.WriteToLog("Could not locate data file in directory: "+datatape_dir, "red")
		return
	end if
	
	' If there is a file code like alpha, bravo, etc then we need to load it in now and flush the variable after
	file_code = VA.GetText("file_code")
	VA.SetText("file_code", nothing)
	
	' If there is no file code then just load the old style datatape.txt file. Otherwise load one of the new style
	if file_code = "" then
		if File.exists(datatape_dir + "\datatape.txt") then
			datatape_file = datatape_dir + "\datatape.txt"
			file_found = true
		end if
	else
		datatape_file = Dir(datatape_dir + "\datatape_" + file_code + "_*.txt")
		if datatape_file <> "" then
			datatape_file = datatape_dir + "\" + datatape_file
			file_found = true
		end if
	end if
	
	' No files were found and this is bad. Show instructions on what to do. 
	if file_found = false then
		VA.WriteToLog("Advanced File Format: datatape_alpha_<ANYTHING_YOU_WANT_HERE>.txt", "red")
		VA.WriteToLog("Basic File Format: datatape.txt", "red")
		VA.WriteToLog("File name must match one of the following formats:", "red")
		VA.WriteToLog("Could not locate compatible datatape file", "red")
		return
	end if
	
	'If a previous file had more steerpoints, we need to clear them out. This will null them
	while pos < 101
		current_pos = VA.GetText("waypoint_"+pos.toString())
		if current_pos <> "" then 
			if VA.getText("latitude_"+pos.toString()) = "20000000" then
				VA.SetText("waypoint_"+pos.toString(), nothing)
				VA.SetText("latitude_"+pos.toString(), nothing)
				VA.SetText("longitude_"+pos.toString(), nothing)
				VA.SetText("altitude_"+pos.toString(), nothing)
			else
				VA.SetText("latitude_"+pos.toString(), "20000000")
				VA.SetText("longitude_"+pos.toString(), "400000000")
				VA.SetText("altitude_"+pos.toString(), "0")
			end if
		end if
		pos = pos + 1
	end while
	
	' reset position so we can clean up some data later
	pos = 2
		
	' Main loop. Read the file, trim some fat, and get it plugged into vars	
	VA.WriteToLog("Reading from datatape file: "+ datatape_file, "blue")
	
	for each line as string in File.ReadLines(datatape_file)
	
		' We can comment out lines by prepending a # 
		if line.contains("#") = false then
		
			' Trim out any comments within [BRACKETS] 
			line = Regex.replace(line, "\s?\[(.*)\]", "")
	
			Dim rowdata As String() = line.Split(New Char() {":"c})

			If rowdata(0) <> "1" Then
			
				' Just in case there are typos, trim them out 
				line = line.replace("	", " ").replace("     ", " ").replace("    ", " ").replace("   ", " ").replace("  ", " ")
				line = line.replace("'","").replace(":","").replace("""", "").replace("FT","").replace("/", " ").replace("\", " ")
			
				Dim values As String() = line.Split(New Char() {" "c})
				if values.length = 4 Then
				
					' Lots of sanity checking going on here
					Dim waypoint as string = values(0)
					Dim latitude as string = values(1)
					Dim longitude as string = values(2)
					Dim altitude as string = values(3)
					
					Dim latitude_parts As String() = latitude.Split(New Char() {"."c})
					Dim longitude_parts As String() = longitude.Split(New Char() {"."c})
					
					Dim latitude_hm as String = latitude_parts(0)
					Dim latitude_sec as String = latitude_parts(1)
					
					Dim longitude_hm as String = longitude_parts(0)
					Dim longitude_sec as String = longitude_parts(1)
					
					' We need to verify the right data is in the E/W coordinates
					If longitude_parts.contains("E0") = False And longitude_parts.contains("W0") = False And longitude_hm.length <> 6 Then
						longitude = longitude.replace("E", "E0").replace("W","W0")
					End if
					
					' Replace NSEW with numbers 
					latitude = latitude.replace("N","2").replace("S","8").replace(".", "")
					longitude = longitude.replace("E","6").replace("W","4").replace(".", "")
					altitude = altitude.replace(",","")
					
					' Append zeroes to any values shorter than required
					while latitude.length < 8
						latitude = latitude + "0"
					end while
					
					while longitude.length < 9
						longitude = longitude + "0"
					end while
					
					' Write to logs and set the variables
					VA.WriteToLog("loading waypoint from file: "+ waypoint + " " + latitude + " " + longitude + " " + altitude, "blue")
					VA.SetText("waypoint_"+waypoint, waypoint)
					VA.SetText("latitude_"+waypoint, latitude)
					VA.SetText("longitude_"+waypoint, longitude)
					VA.SetText("altitude_"+waypoint, altitude)
				End if
				
				' increment position for later
				pos = pos + 1
				
			End If
		end if
	Next
	
	End Sub

End Class
