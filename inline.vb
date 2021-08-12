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
Imports System.Web.Script.Serialization
Imports System.Xml
Imports System.Xml.Serialization

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
		Dim xml_file_found as Boolean = false
		Dim datatape_file as string
		
		Dim file_code as string
		
		Dim pos as integer = 2
		Dim current_pos as string
		
		' Clear all variabls firs
		while pos < 101
			VA.SetText("waypoint_"+pos.toString(), nothing)
			VA.SetText("latitude_"+pos.toString(), nothing)
			VA.SetText("longitude_"+pos.toString(), nothing)
			VA.SetText("altitude_"+pos.toString(), nothing)
			pos = pos + 1
		end while
		
		pos = 2
		
		
		' Look for the available directories and pick the one that works the best based on the order within the list
		while dir_found = false and dir_loc < dir_locations.length
			
			If Directory.Exists(dir_locations(dir_loc)) Then
				dir_found = true
				datatape_dir = dir_locations(dir_loc)
				VA.WriteToLog("Reading data from directory: "+datatape_dir, "blue")
			End If
			dir_loc = dir_loc + 1
			
		end while
		
		' If no directory was found, then it needs to be created/fixed. Output some insturctions on how to do so.
		If dir_found = false then
			If file_code <> nothing then
				VA.WriteToLog("Advanced File Format: datatape_"+file_code+"_<ANYTHING_YOU_WANT_HERE>.txt", "red")
			Else
				VA.WriteToLog("Advanced File Format: datatape_alpha_<ANYTHING_YOU_WANT_HERE>.txt", "red")
			End If
			VA.WriteToLog("Basic File Format: datatape.txt", "red")
			VA.WriteToLog("File name must match one of the following formats:", "red")
			VA.WriteToLog(dir_locations(2), "red")
			VA.WriteToLog(dir_locations(1), "red")
			VA.WriteToLog(dir_locations(0), "red")	
			VA.WriteToLog("In order for datatape to run, you need to create a datatape file in one of the following locations:", "red")	
			VA.WriteToLog("Could not locate data file in directory: "+datatape_dir, "red")
			return
		End If
		
		' If there is a file code like alpha, bravo, etc then we need to load it in now and flush the variable after
		file_code = VA.GetText("file_code")
		VA.SetText("file_code", nothing)
		
		' If there is no file code then just load the old style datatape.txt file. Otherwise load one of the new style
		' Prefer XML files and fall back to TXT files
		If file_code = "" then
			If File.exists(datatape_dir + "\datatape.xml") then
				datatape_file = datatape_dir + "\datatape.xml"
				file_found = true
				xml_file_found = true
			Else
				If File.exists(datatape_dir + "\datatape.txt") then
					datatape_file = datatape_dir + "\datatape.txt"
					file_found = true
				End If
			End If
		Else
			datatape_file = Dir(datatape_dir + "\datatape_" + file_code + "*.xml")
			If datatape_file <> "" then
				datatape_file = datatape_dir + "\" + datatape_file
				file_found = true
				xml_file_found = true
			Else 
				datatape_file = Dir(datatape_dir + "\datatape_" + file_code + "*.txt")
				If datatape_file <> "" then
					datatape_file = datatape_dir + "\" + datatape_file
					file_found = true
				End If
			End If 
		End If
		
		' No files were found and this is bad. Show instructions on what to do. 
		If file_found = false then
			If file_code <> nothing then
				VA.WriteToLog("Advanced File Format: datatape_"+file_code+"_<ANYTHING_YOU_WANT_HERE>.txt", "red")
			Else
				VA.WriteToLog("Advanced File Format: datatape_alpha_<ANYTHING_YOU_WANT_HERE>.txt", "red")
			End If
			VA.WriteToLog("Basic File Format: datatape.txt", "red")
			VA.WriteToLog("File name must match one of the following formats:", "red")
			VA.WriteToLog("Could not locate compatible datatape file", "red")
			return
		End If
			
		' Main loop. Read the file, trim some fat, and get it plugged into vars	
		VA.WriteToLog("Reading from datatape file: "+ datatape_file, "blue")
		
		' Run a dIfferent loop for XML files.
		
		If xml_file_found = true then
		
			Dim doc As New XmlDocument()
			doc.Load(datatape_file)
			Dim xml As String = File.ReadAllText(datatape_file)
		
			Dim stpt as integer = 1
			
			Dim sSerialize As Serializer = New Serializer()
			
			Dim objects As Objects = sSerialize.Deserialize(Of Objects)(xml)
			
			
			
			For Each xml_waypoint As Waypoint In objects.Waypoints
				If stpt > 1 then											
						Dim formatted_lat as decimal = Math.Round(xml_waypoint.position.latitude, 6, MidpointRounding.AwayFromZero)
						Dim formatted_lng as decimal = Math.Round(xml_waypoint.position.longitude, 6, MidpointRounding.AwayFromZero)
									
						Dim latitude as string = ConvertLatToDM(formatted_lat)
						Dim longitude as string = ConvertLongToDM(formatted_lng)
						Dim waypoint as string = stpt
						Dim altitude as string = ConvertMetersToFeet(xml_waypoint.position.altitude)
						' Write to logs and set the variables
						VA.WriteToLog("loading waypoint from file: "+ waypoint + " " + latitude + " " + longitude + " " + altitude, "blue")
						VA.SetText("waypoint_"+waypoint, waypoint)
						VA.SetText("latitude_"+waypoint, latitude)
						VA.SetText("longitude_"+waypoint, longitude)
						VA.SetText("altitude_"+waypoint, altitude)
				End if
				stpt = stpt + 1
			Next
				

												
			'For Each item As Dictionary(Of String, Object) In data("drawings")
			'
			'	If item("type") = "circle" then
			'		If stpt > 1 then 
			'			VA.WriteToLog("Auto Loading Steerpoint "+stpt.toString()+"- " + item("name"), "green")
			'			
			'			Dim lat as decimal = item("center_y")
			'			Dim formatted_lat as decimal = Math.Round(lat, 6, MidpointRounding.AwayFromZero)
			'			
			'			Dim lng as decimal = item("center_x")
			'			Dim formatted_lng as decimal = Math.Round(lng, 6, MidpointRounding.AwayFromZero)
			'						
			'			Dim latitude as string = ConvertLatToDM(formatted_lat)
			'			Dim longitude as string = ConvertLongToDM(formatted_lng)
			'			Dim waypoint as string = stpt
			'			Dim altitude as string = "0"
			'			' Write to logs and set the variables
			'			VA.SetText("waypoint_"+waypoint, waypoint)
			'			VA.SetText("latitude_"+waypoint, latitude)
			'			VA.SetText("longitude_"+waypoint, longitude)
			'			VA.SetText("altitude_"+waypoint, altitude)
			'		End If
			'		stpt = stpt + 1
			'		
			'	End If
			'
			'Next
			
		
		Else 
			for each line as string in File.ReadLines(datatape_file)
			
				' We can comment out lines by prepending a # 
				If line.contains("#") = false then
				
					' Trim out any comments within [BRACKETS] 
					line = Regex.replace(line, "\s?\[(.*)\]", "")
			
					Dim rowdata As String() = line.Split(New Char() {":"c})

					If rowdata(0) <> "1" Then
					
						' Just in case there are typos, trim them out 
						line = trim(line)
						
						line = Regex.Replace(line, " {2,}", " ")
						line = line.replace("'","").replace(":","").replace("""", "").replace("FT","").replace("/", " ").replace("\", " ")
					
						Dim values As String() = line.Split(New Char() {" "c})
						If values.length = 4 Then
						
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
							
							' We need to verIfy the right data is in the E/W coordinates
							If longitude_parts.contains("E0") = False And longitude_parts.contains("W0") = False And longitude_hm.length <> 6 Then
								longitude = longitude.replace("E", "E0").replace("W","W0")
							End If
							
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
						End If
						
						' increment position for later
						pos = pos + 1
						
					End If
				End If
			Next
		End If
	End Sub
	
	Function ConvertMetersToFeet(ByVal mm As decimal) As string
	
		
		Dim result as decimal = Math.Round(mm * 3.28084)		
		Dim feet as integer = Math.Truncate(result)
		
		ConvertMetersToFeet = feet.toString
		
	End Function
		
	
	Function ConvertLatToDM(ByVal dd As decimal) As string
		
		Dim result as string = ""
		Dim deg as integer = Math.Truncate(dd)
		Dim fraction as decimal = Math.Abs(dd) - Math.Abs(deg)		
		
		If deg > 0 then
				result = "2"
		Else	
				deg = Math.Abs(deg)
				result = "8"
				
		End If
		
		result = result + deg.toString()
		
		Dim minutes as decimal = fraction * 60
		minutes = Math.Round(minutes, 3, MidpointRounding.AwayFromZero)
		If minutes < 10 then
			result = result + "0"
		End If
		
		minutes = Math.Round(minutes*1000, 0, MidpointRounding.AwayFromZero)
		
		ConvertLatToDM = result + minutes.toString()
	End Function
	
	Function ConvertLongToDM(ByVal dd As decimal) As string
	
		Dim result as string = ""
		Dim deg as integer = Math.Truncate(dd)
		Dim fraction as decimal = Math.Abs(dd) - Math.Abs(deg)	
	
		If deg > 0 then
				result = "6"
		Else	
				deg = Math.Abs(deg)
				result = "4"
		End If	
		
		If deg < 100 then
			result = result + "0"
		End If
				
		result = result + deg.toString()
		
		Dim minutes as decimal = fraction * 60
		minutes = Math.Round(minutes, 3, MidpointRounding.AwayFromZero)
		If minutes < 10 then
			result = result + "0"
		End If
		
		minutes = Math.Round(minutes*1000, 0, MidpointRounding.AwayFromZero)
		
		ConvertLongToDM = result + minutes.toString()
	End Function

End Class



Public Class Objects
 
   Public Property Waypoints As List(Of Waypoint)
 
End Class


Public Class Waypoint
 
   Public Property Name As String
   Public Property Position As Position
 
End Class

Public Class Position
 
   Public Property Latitude As decimal
   Public Property Longitude As decimal
   Public Property Altitude As decimal
 
End Class

Public Class Serializer
   Public Function Deserialize(Of T As Class) _
         (ByVal input As String) As T
 
      Dim ser As XmlSerializer = New XmlSerializer(GetType(T))
 
      Using sr As StringReader = New StringReader(input)
 
         Return CType(ser.Deserialize(sr), T)
 
      End Using
 
   End Function
 
   Public Function Serialize(Of T)(ByVal ObjectToSerialize As T) _ 
   As String
 
      Dim xmlSerializer As XmlSerializer = New XmlSerializer(ObjectToSerialize.[GetType]())
 
      Using textWriter As StringWriter = New StringWriter()
 
         xmlSerializer.Serialize(textWriter, ObjectToSerialize)
 
         Return textWriter.ToString()
 
      End Using
 
   End Function
 
End Class
