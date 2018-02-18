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
	/*$sql = "INSERT INTO test (Name) VALUES ('{$name}')";

		if ($conn->query($sql) === TRUE) {
	    echo "worked";
		} else {
	    echo "Error: " . $sql . "<br>" . $conn->error;
		}*/
	$sql = "SELECT * FROM Users";
	$result = $conn->query($sql);
	if ($result->num_rows > 0) {
	    while($row = $result->fetch_assoc()) {
	    	echo $row["FirstName"];
	    }
	}
?>