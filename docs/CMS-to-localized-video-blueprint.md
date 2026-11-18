# CMS release note to localized video

Build a release-note walkthrough in HeyGen from a Contentful entry. Route each translated SRT to a linguist and generate a dubbed video after they approve it. This is a build guide for Blackbird Designer. The app must be installed in the target Nest before the new nodes can be selected.

## Connections and sample content

| Connection | Required setup |
|---|---|
| Contentful | Access to the source space and environment. Use a published release-note content type in one source locale, such as `en-US`. Tag video-ready entries `video-ready`. |
| HeyGen 1.3.0 | An API workspace with funds for Studio rendering and proofreading. The key needs `videos:write`, `translations:write`, and `assets:write` for approved SRT uploads and any Blackbird media uploads. Give the connection access to the avatar look and voice. |
| Dropbox | Separate review, approval, and output folders that the connection can access. Grant linguists write access to the approval folder. Use a dedicated path for this Bird. |

Use a plain-text field for the intro, another for screenshot narration, a screenshot asset, and a demo video asset. Record the CMS entry ID and publication revision. The walkthrough is a new output file. The Bird does not need to edit or republish the CMS entry.

## Workflow steps and bindings

| Step | Blackbird node | Bindings and routing |
|---|---|---|
| 1 | Contentful **On entry published** | Filter to the intended source locale and `video-ready` tag. During development, use a manual trigger with a known entry ID. |
| 2 | Contentful **Get entry** and field reads | Bind the entry ID from the trigger. Read the title, intro script, screenshot narration, screenshot asset, and demo asset in the source locale. For a protected media source, download it as a Blackbird file or supply a HeyGen asset ID. Provider-fetched URLs must be directly downloadable. |
| 3 | HeyGen **Create Studio video** | Title from the source entry. Bind an ordered list of three scenes using the example below. Use a stable key such as `entry123:revision7:studio-v1`. Use a short sample for the first test. |
| 4 | Check original video readiness | Use **Get video** with the returned video ID. For `pending` or `processing`, add a Delay before checking again in a bounded branch. Follow the provider's failure details if the job fails. For `completed`, continue using the same video ID. |
| 5 | HeyGen **Start translation proofread** | Source video ID from step 3. Choose explicit target languages and pass a distinct proofread submission key, such as `entry123:revision7:proofread-v1`. |
| 6 | Loop over **Proofread session IDs** | Each iteration owns one session. Use a polling Checkpoint **On proofread ready** filtered to the current session ID. Read its language from the ready session or with **Get proofread session**. |
| 7 | HeyGen **Download proofread SRT**, then Dropbox **Upload file** | Use the current session ID. Save the draft to the review folder with a session-specific name such as `<proofreadId>--r1.srt`. |
| 8 | Collect approval | The linguist edits and uploads the approved SRT to the separate approval folder using the same session and revision name. Use a Dropbox **On files updated** event Checkpoint for the approval folder when supported by the Nest. Filter or branch on the full expected path before proceeding. If exact path scoping is unavailable, use a separate approval Bird with a file-update trigger and validate the folder, session ID and expected current revision. Ignore unmatched files. |
| 9 | Dropbox **Download file**, then HeyGen **Upload edited proofread SRT** | Download the approved file by its ID or exact path. Bind its Blackbird file and the matching session ID. A replacement must finish before final generation starts. Do not upload an older revision over a newer approved file. |
| 10 | HeyGen **Generate video from proofread** | Map the proofread session ID from the upload. Use an idempotency key scoped to the approved revision, such as `<proofreadId>:r1:final`. |
| 11 | HeyGen **Get video translation**, **Download translated video** | Map step 10's **Translation ID**. Wait for `completed` using bounded Delay and status checks. Route failed renders for review. Save the MP4 and optional **Get translation captions** output to the designated output folder. |

A proofread session and its final translation have different IDs. Keep the proofread ID with the SRT and carry the translation ID to the final status and download steps. Keep source video, review, and published output folders distinct to prevent workflow loops.

## Map the three scene objects

The following example shows the values to bind to the **Scenes** input. Use the action editor’s structured array binding, or have an upstream transformation serialize this array into **Scenes JSON**. Use the structured input for Blackbird files. This snippet uses the connector model property names.

```json
[
  {
    "Type": "avatar_video",
    "AvatarId": "<approved avatar look ID>",
    "Script": "<release-note intro text>"
  },
  {
    "Type": "image",
    "MediaUrl": "<direct screenshot URL>",
    "Script": "<screenshot narration>",
    "VoiceId": "<narration voice ID>"
  },
  {
    "Type": "video",
    "MediaUrl": "<direct demo clip URL>",
    "MuteClip": false
  }
]
```

For an uploaded audio file from **Text to speech**, bind it to **Narration file** and leave **Script** and **Voice ID** empty on that scene. For an unvoiced screenshot, provide **Still-image duration (seconds)** and omit narration. Use a stable source revision and scene order so retries reference the same work.

## Review before activation

Run one short release in one target language. Confirm the rendered order, the approved SRT file mapping, and the dubbed version. Use the same final submission key to test a repeat of the same approved revision. Check language and file naming before enabling the Contentful trigger. If a proofread is already complete when its scoped Checkpoint starts, the first poll continues the Flight. An unfiltered workspace trigger begins with a baseline and skips already-ready sessions.

The Blackbird connection available during this build did not provide an active organization. The guide and scene example were prepared without a deployment or a verified importable Bird export. Select the actual Contentful fields, connections, and Dropbox folders in your Nest.

[Contentful connector](https://docs.blackbird.io/apps/contentful/) · [Dropbox connector](https://docs.blackbird.io/apps/dropbox/) · [Checkpoints](https://docs.blackbird.io/checkpoints/) · [HeyGen Studio](https://developers.heygen.com/studio-videos)
