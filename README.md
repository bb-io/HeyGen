# Blackbird.io HeyGen

Blackbird is the new automation backbone for the language technology industry. Blackbird provides enterprise-scale automation and orchestration with a simple no-code/low-code platform. Blackbird enables ambitious organizations to identify, vet and automate as many processes as possible. Not just localization workflows, but any business and IT process. This repository represents an application that is deployable on Blackbird and usable inside the workflow editor.

## Introduction

<!-- begin docs -->

HeyGen is an AI video platform for avatar videos, voice-cloned dubbing and lip-sync. Use this app to generate avatar videos from scripts or Studio templates, translate existing footage with native HeyGen dubbing, or route editable scripts, template text and translated subtitles through a TMS or a human review checkpoint before rendering a new video.

## Before setting up

Before you can connect you need to make sure that:

- You have a HeyGen account with API access and a funded API balance. Most generation actions (videos, translations, lipsync, clipping, speech) consume API credits. See the [HeyGen API pricing](https://help.heygen.com/en/articles/10060327-heygen-api-pricing-credits-explained) and your [account pricing page](https://platform.heygen.com/pricing).
- You have created an API key in the [HeyGen API dashboard](https://platform.heygen.com/api-keys), in the workspace that owns or can access your videos and assets. Enable only the [permissions](https://developers.heygen.com/docs/api-key-permissions) your workflows need.

## Connecting

1. Navigate to **Apps** and search for **HeyGen**.
2. Click **Add Connection**.
3. Name your connection for future reference, e.g. 'My HeyGen connection'.
4. Fill in the **API key** you created in the HeyGen API dashboard.
5. Click **Connect**.
6. Confirm that the connection has appeared and the status is **Connected**.

## Actions

### Videos

- **Search videos** finds accessible HeyGen videos by title and status.
- **Get video** returns the current status, metadata and download URLs for a video.
- **Download video** downloads the rendered video as a file.
- **Get captions** downloads the SRT captions of a rendered video.
- **Create video** submits a single-scene avatar video from a script and returns its video ID.

### Studio video

- **Create Studio video** composes 1–50 ordered avatar, image and video scenes into one render. Media and narration can be HeyGen asset IDs, directly downloadable URLs or Blackbird files. Use the returned video ID with the video status and download actions.

### Templates

- **Get template variables** returns the editable variable names, types and defaults of an API-ready template.
- **Create video from template** fills an API-ready Studio template and submits a new render.
- **Export template text as XLIFF** exports the current default values of a template's text variables for translation.
- **Create template video from XLIFF** applies the translated XLIFF text to the same template and submits a new render.

### Script localization

- **Export video script as XLIFF** exports the editable synthesized speech of a HeyGen video's scenes for translation.
- **Read translated video script** reads translated speech from an exported XLIFF, ready to use in **Create video**.

### Audio

- **Text to speech** turns written text into a WAV or MP3 narration file using a HeyGen voice.
- **Speech to speech** revoices a recording with an instant voice from your workspace while preserving its words and timing.
- **Create avatar video from audio** submits an avatar render using a narration file, an HTTPS URL or a HeyGen audio asset ID.

### Video translation

- **Translate video** starts native HeyGen voice-cloned dubbing and returns a translation ID for each requested language.
- **Get video translation** returns a translation job's status, output language and artifact URLs.
- **Download translated video** downloads the translated video as a file.
- **Get translation captions** downloads the translated SRT or WebVTT captions.

### Translation proofreading

- **Start translation proofread** prepares an editable translated SRT and returns a proofread session ID for each language.
- **Get proofread session** returns the session status, language and failure details.
- **Download proofread SRT** downloads the current translated SRT from a completed proofread session.
- **Upload edited proofread SRT** replaces the session's translated subtitles with the reviewed SRT.
- **Generate video from proofread** renders the dubbed video using the current proofread subtitles and returns a translation ID.

A typical proofreading workflow: start a proofread session for each target language, wait with **On proofread ready** scoped to the session, download the SRT, send it to a linguist through a checkpoint or your TMS, upload the edited SRT and generate the dubbed video. See the [HeyGen proofreading workflow](https://developers.heygen.com/docs/video-translate#proofread-before-finalizing).

### Video Agent

- **Create Video Agent session** generates a video from a prompt and optional reference files.
- **Get Video Agent session** returns the session status and the video ID once assigned.
- **List Video Agent drafts** lists the videos produced by a Video Agent session, newest first.
- **List video scenes** returns the scene IDs, narration and edit version of a draft.
- **Revise selected scenes** submits one instruction per selected scene and returns the new draft's video ID when available.

### Lipsync

- **Create lipsync** replaces the audio of a video and submits it for lip re-animation.
- **Get lipsync** returns a lipsync job's status and artifact URLs.
- **Download lipsync video** downloads the completed lipsync render as a file.

### AI Clipping

- **Create AI clipping job** generates short highlight clips from a source video.
- **Get AI clipping job** returns the job progress and the produced clips.
- **Download clip** downloads a completed highlight clip.

### Batches

- **Create batch** submits 1–100 video, translation or lipsync requests as a native HeyGen batch.
- **Get batch** returns the aggregate status and individual outcomes of a batch.

### Asset batches

- **Create asset batch** uploads files into a native HeyGen asset batch and starts ingestion.
- **Complete asset batch** repeats ingestion for an asset batch after all files have been uploaded.

### Brand glossaries

- **Search brand glossaries** finds the brand glossaries visible to the connection.
- **Get brand glossary** returns the pronunciation, do-not-translate and forced-translation rules of a glossary.
- **Create brand glossary** creates a glossary with optional pronunciation and translation rules.
- **Update brand glossary** changes the name or replaces the supplied rule lists. Omitted rule lists remain intact.
- **Delete brand glossary** permanently deletes a glossary.

## Events

- **On video completed** triggers when HeyGen videos are completed.
- **On video failed** triggers when HeyGen videos fail.
- **On translation completed** triggers when native video translations are completed.
- **On proofread ready** triggers when translated subtitles of a proofread session are ready for editing. Scope it to one session to use it as a checkpoint.

All events are polling events. The first poll records existing items without starting workflows. Results found in one poll are sent together.

## Limitations

- For endpoints that support idempotency, provide a **Submission key** unique to the logical job (for example `asset-123:es:revision-7`). Reuse it only for the same job and inputs within HeyGen's 24-hour window.
- Rate-limited (HTTP 429) reads are retried automatically; writes are not. If a write fails with an unclear result, check the job history in HeyGen before running it again.
- HeyGen applies rate and concurrency limits per plan. See [HeyGen usage limits](https://developers.heygen.com/docs/usage-limits).

## Feedback

Do you want to use this app or do you have feedback on our implementation? Reach out to us using the [established channels](https://www.blackbird.io/) or create an issue.

<!-- end docs -->
