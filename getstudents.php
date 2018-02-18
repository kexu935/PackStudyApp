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
	//get each of the students from the courses
	$sql = "SELECT * FROM Enrollment WHERE Course = '{$Course}'";
	//$sql = "SELECT * FROM Enrollment WHERE Course = 'CS105'";
	$result = $conn->query($sql);
	$ids = '';
	$counter = 1;
     if ($result->num_rows > 0) {
      while($row = $result->fetch_assoc()) {
        if($counter == $result->num_rows)
        {
         $ids .= $row["UserId"];
        }
        else
        {
          $ids .= $row["UserId"].',';
        }
        $counter += 1;
      }
     }
	//select the students by their id
	$studentIdQuery = "SELECT * FROM Users WHERE id IN ($ids)";
      $sql = $studentIdQuery;
      $result = $conn->query($sql);
      if ($result->num_rows > 0) {
      while($row = $result->fetch_assoc()) {
        $rows[] = $row;
      }
      print(json_encode($rows, JSON_NUMERIC_CHECK));
    }
	
?>