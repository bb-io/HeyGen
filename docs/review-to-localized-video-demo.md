# Review one scene, then localize the revised video

This guide configures a Bird in Blackbird Designer. The available Blackbird session did not have an active organization, so the guide has no deployed Bird export or live execution result.

## Start with a short completed Video Agent draft

Install HeyGen 1.4.0 in the test Nest. Use a disposable HeyGen session with a completed draft and an API key with `video_agent:read`, `video_agent:write`, `videos:read`, `translations:read`, and `translations:write`. An edited SRT supplied as a Blackbird file also needs `assets:write`.

| Step | Node | Binding or check |
|---|---|---|
| 1 | **List Video Agent drafts** | Use the Agent session ID. Choose a specific completed draft so a concurrent edit cannot accidentally change the review target. |
| 2 | **List video scenes** | Map the chosen draft’s video ID. Present the ordered scene numbers and scripts to the reviewer. |
| 3 | Review approval | Record the draft video ID, edit version, scene ID, and reviewed instruction together. For example, choose scene three and ask HeyGen to replace an outdated claim while keeping its visuals. |
| 4 | **Revise selected scenes** | Submit the Agent session ID, reviewed draft ID, edit version, scene ID and instruction. Use the review record and revision as the submission key. Supply a distinct key for a new approval. |
| 5 | **Get video** and **Download video** | Use the newly returned **Video ID**. Wait for Completed with bounded status checks. If the submission response has no Video ID, identify the resulting draft in the session before continuing. Stop on a stale version and get a fresh review. |
| 6 | Approve the revised cut | Compare it with the previous cut, verify the revised statement, and check two untouched scenes. Use the approved draft ID in the next step. |
| 7 | **Start translation proofread** | Submit the approved draft and one comparison language. Loop on the returned proofread IDs if using more than one language. |
| 8 | **On proofread ready**, then **Download proofread SRT** | Scope the checkpoint to the proofread ID and give the resulting SRT to the linguist. |
| 9 | **Upload edited proofread SRT**, then **Generate video from proofread** | Use only the matching session’s approved SRT. Use a stable key for this final approved revision. |
| 10 | **Get video translation** and **Download translated video** | Use the returned **Translation ID**. Confirm the video arrives as a Blackbird file and compare the approved wording with the final captions. |

Keep the pre-edit draft, new draft, proofread session, and final translation IDs as distinct workflow fields. On a stale edit version, list the latest scenes again and have the reviewer confirm the updated target. Keep edits within a session sequential.

## Release acceptance checks

- The draft history includes both cuts and the new draft has its own Video ID.
- An unchanged retry with the same submission key does not create another revision.
- The revised cut contains the approved change and can be downloaded as a Blackbird file.
- Proofreading runs on the revised draft. The approved SRT stays paired with its target language and the final dubbed video downloads successfully.
- In a separate Studio test Bird, select **Automatic per Flight**, bind distinct render slots to two renders in one Flight, and rerun a single render using the same Flight and slot. Confirm the keys differ for the two slots and stay stable for the retry.
