const form = document.getElementById("uploadForm");
const message = document.getElementById("message");
const fileInput = document.getElementById("files");

form.addEventListener("submit", async (e) => {
  e.preventDefault();

  message.textContent = "Uploading...";
  message.classList.add("loading");

  const formData = new FormData(form);

  try {
    const response = await axios.post(
      "https://localhost:7094/api/jobs/upload",
      formData,
      { headers: { "Content-Type": "multipart/form-data" } }
    );

    // הסרת טעינה
    message.classList.remove("loading");

    // הצגת הצלחה עם אנימציה קצרה
    message.textContent = "הטופס הועלה בהצלחה!";
    message.classList.add("success");

    // אחרי האנימציה מחליפים למחלקה רגילה
    setTimeout(() => {
      message.classList.remove("success");
    }, 1600);

    console.log("Job uploaded successfully:", response.data);

  } catch (error) {
    message.classList.remove("loading");
    message.textContent = `Upload failed: ${error.response?.data?.title || error.message}`;
    console.error("Error uploading job:", error.response || error);
  }
});
