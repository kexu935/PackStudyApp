<?php
	$ini_array = parse_ini_file("path.ini");
  	$path = $ini_array["URL"];
  	$host = $ini_array["HOST"];
  	$password = $ini_array["PASSWORD"];
	$dbusername = $ini_array["USERNAME"];
  	$dname = $ini_array["DNAME"];

  	//connecting to database
 	$conn = mysqli_connect($host, $dbusername, $password, $dname);
	if(mysqli_connect_error())
	{
		die("Could not connect to database");
	}
	$Course = $_POST["Course"];
	$LastId = $_POST["LastId"];
	
	//check if email iis in the User database
	$sql = "SELECT * FROM {$Course} WHERE id > {$LastId}";
	$result = $conn->query($sql);
    if ($result->num_rows > 0) {
      while($row = $result->fetch_assoc()) {
        $rows[] = $row;
      }
      print(json_encode($rows, JSON_NUMERIC_CHECK));
    }
	
?>