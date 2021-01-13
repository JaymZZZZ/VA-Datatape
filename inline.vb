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


Public Class VAInline


	Public Sub Main()
	
	Dim file_locations = {
	Environ$("USERPROFILE")+"\Saved Games\DCS.openbeta\Datatape\datatape.txt", 
	Environ$("USERPROFILE")+"\Saved Games\DCS\Datatape\datatape.txt", 
	"C:\Program Files (x86)\VoiceAttack\Datatape\datatape.txt"
	}
	
	Dim file_found as Boolean = false
	Dim datatape_file as string
	Dim loc as integer = 0
	
	while file_found = false and loc < file_locations.length
		
		If File.Exists(file_locations(loc)) Then
			file_found = true
			datatape_file = file_locations(loc)
			VA.WriteToLog("Reading data from: "+datatape_file, "blue")
		end if
		loc = loc + 1
		
	end while
	
	if file_found = false then
		VA.WriteToLog(file_locations(2), "red")
		VA.WriteToLog(file_locations(1), "red")
		VA.WriteToLog(file_locations(0), "red")	
		VA.WriteToLog("In order for datatape to run, you need to create a datatape file in one of the following locations:", "red")	
		VA.WriteToLog("Could not locate data file: "+datatape_file, "red")
		return
	end if
	
	for each line as string in File.ReadLines(datatape_file)

		If line.contains("1:") = False Then
		
			line = line.replace("	", " ").replace("     ", " ").replace("    ", " ").replace("   ", " ").replace("  ", " ")
			line = line.replace("'","").replace(":","").replace("""", "").replace("FT","").replace("/", " ").replace("\", " ")
		
			Dim values As String() = line.Split(New Char() {" "c})
			if values.length = 4 Then
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
				
				If longitude_parts.contains("E0") = False And longitude_parts.contains("W0") = False And longitude_hm.length <> 6 Then
					longitude = longitude.replace("E", "E0").replace("W","W0")
				End if
				
				latitude = latitude.replace("N","2").replace("S","8").replace(".", "")
				longitude = longitude.replace("E","6").replace("W","4").replace(".", "")
				altitude = altitude.replace(",","")
				
				while latitude.length < 8
					latitude = latitude + "0"
				end while
				
				while longitude.length < 9
					longitude = longitude + "0"
				end while
				
				VA.WriteToLog("loading waypoint: "+ waypoint + " " + latitude + " " + longitude + " " + altitude, "blue")
				VA.SetText("waypoint_"+waypoint, waypoint)
				VA.SetText("latitude_"+waypoint, latitude)
				VA.SetText("longitude_"+waypoint, longitude)
				VA.SetText("altitude_"+waypoint, altitude)
			End if
			
		End If
	Next
	
	End Sub

End Class
